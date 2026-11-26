using System.Collections.Concurrent;

namespace MantenimientoFlota.Api.Notifications;

/// <summary>Conexiones suscritas; no almacena tokens ni datos de inventario.</summary>
public sealed class StockSubscriptions
{
    private readonly ConcurrentDictionary<string, StockSubscriber> _connections = new();

    public void Registrar(string connectionId, int usuarioId, string rol, string login) =>
        _connections[connectionId] = new StockSubscriber(usuarioId, rol, login);

    public void Quitar(string connectionId) => _connections.TryRemove(connectionId, out _);

    public KeyValuePair<string, StockSubscriber>[] Obtener() => _connections.ToArray();
}

public sealed record StockSubscriber(int UsuarioId, string Rol, string Login);
