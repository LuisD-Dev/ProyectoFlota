using MantenimientoFlota.Api.Authorization;
using MantenimientoFlota.Api.Notifications;
using MantenimientoFlota.Domain.Constants;
using MantenimientoFlota.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MantenimientoFlota.Api.Hubs;

/// <summary>Conexiones autorizadas para recibir alertas de inventario.</summary>
[Authorize(Policy = AuthorizationPolicies.AdministradorOCoordinador)]
public sealed class StockHub(IUsuarioRepository usuarios, StockSubscriptions subscriptions) : Hub
{
    public const string Ruta = "/hubs/stock";
    public const string GrupoResponsables = "responsables-stock";
    public const string EventoStockBajo = "StockBajoDetectado";
    public const string EventoSesionRevocada = "StockSesionRevocada";

    public async Task SuscribirseStock()
    {
        if (!int.TryParse(Context.User?.FindFirst("sub")?.Value, out var id))
            throw new HubException("No se pudo identificar al usuario.");

        var usuario = await usuarios.GetByIdAsync(id);
        if (usuario is null || !usuario.Activo ||
            usuario.Rol.Nombre is not (NombresRol.Administrador or NombresRol.CoordinadorTecnico) ||
            usuario.Rol.Nombre != Context.User?.FindFirst("role")?.Value ||
            usuario.Login != Context.User?.FindFirst("name")?.Value)
            throw new HubException("La sesión ya no tiene permiso para consultar inventario.");

        await Groups.AddToGroupAsync(Context.ConnectionId, GrupoResponsables);
        subscriptions.Registrar(Context.ConnectionId, id, usuario.Rol.Nombre, usuario.Login);
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        subscriptions.Quitar(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}
