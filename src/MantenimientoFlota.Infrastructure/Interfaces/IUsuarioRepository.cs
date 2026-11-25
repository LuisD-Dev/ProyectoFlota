using MantenimientoFlota.Domain.Entities;

namespace MantenimientoFlota.Infrastructure.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> GetByLoginAsync(string login);
    Task<IReadOnlyList<Usuario>> GetAllAsync();
    Task<Usuario?> GetByIdAsync(int id);
    Task<IReadOnlyList<Rol>> GetRolesAsync();
    Task<Usuario> CreateAsync(Usuario usuario);
    Task UpdateAsync(Usuario usuario);
    Task DesactivarAsync(int id);
    Task<string?> GetRoleNameAsync(int rolId);
    Task RegistrarAccesoAsync(
    int usuarioId,
    DateTime fechaAccesoUtc,
    string? nuevoPasswordHash);
}
