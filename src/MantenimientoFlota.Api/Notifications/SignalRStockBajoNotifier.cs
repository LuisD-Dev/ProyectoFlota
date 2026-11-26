using MantenimientoFlota.Api.Hubs;
using MantenimientoFlota.Application.Interfaces;
using MantenimientoFlota.Domain.DTO.Repuesto;
using MantenimientoFlota.Domain.Entities;
using MantenimientoFlota.Infrastructure.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace MantenimientoFlota.Api.Notifications;

/// <summary>Publica alertas a los responsables conectados mediante SignalR.</summary>
public sealed class SignalRStockBajoNotifier(
    IHubContext<StockHub> hubContext,
    StockSubscriptions subscriptions,
    IUsuarioRepository usuarios,
    ILogger<SignalRStockBajoNotifier> logger)
    : IStockBajoNotifier
{
    public async Task PublicarAsync(StockBajoDto alerta, CancellationToken cancellationToken = default)
    {
        try
        {
            // Un WebSocket conserva los claims del inicio. Comprobar su vigencia
            // antes de publicar evita seguir notificando una cuenta desactivada.
            var revisados = new Dictionary<int, Usuario?>();
            foreach (var (connectionId, suscriptor) in subscriptions.Obtener())
            {
                if (!revisados.TryGetValue(suscriptor.UsuarioId, out var usuario))
                {
                    usuario = await usuarios.GetByIdAsync(suscriptor.UsuarioId);
                    revisados[suscriptor.UsuarioId] = usuario;
                }

                if (usuario is not null && usuario.Activo &&
                    usuario.Rol.Nombre == suscriptor.Rol && usuario.Login == suscriptor.Login)
                    continue;

                await hubContext.Groups.RemoveFromGroupAsync(
                    connectionId, StockHub.GrupoResponsables, cancellationToken);
                subscriptions.Quitar(connectionId);
                await hubContext.Clients.Client(connectionId).SendAsync(
                    StockHub.EventoSesionRevocada, cancellationToken);
            }

            await hubContext.Clients.Group(StockHub.GrupoResponsables).SendAsync(
                StockHub.EventoStockBajo, alerta, cancellationToken);

            logger.LogInformation(
                "Alerta de stock bajo publicada. EventoId={EventoId} RepuestoId={RepuestoId}",
                alerta.EventoId, alerta.RepuestoId);
        }
        catch (Exception exception)
        {
            // La transacción ya se confirmó: el fallo de entrega no revierte el consumo.
            logger.LogWarning(
                "No se pudo publicar la alerta. EventoId={EventoId} RepuestoId={RepuestoId} ExceptionType={ExceptionType}",
                alerta.EventoId, alerta.RepuestoId, exception.GetType().FullName);
        }
    }
}
