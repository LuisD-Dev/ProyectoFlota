using MantenimientoFlota.Domain.Entities;

namespace MantenimientoFlota.Infrastructure.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> GetByLoginAsync(string login);
    Task<string?> GetRoleNameAsync(int rolId);
    Task RegistrarAccesoAsync(
    int usuarioId,
    DateTime fechaAccesoUtc,
    string? nuevoPasswordHash);
}