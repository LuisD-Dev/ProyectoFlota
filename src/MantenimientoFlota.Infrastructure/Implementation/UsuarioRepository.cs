using MantenimientoFlota.Domain.Entities;
using MantenimientoFlota.Infrastructure.Data;
using MantenimientoFlota.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MantenimientoFlota.Infrastructure.Implementation;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> GetByLoginAsync(string login)
    {
        var result = await _context.Set<Usuario>()
            .AsNoTracking()
            .FirstOrDefaultAsync(usuario => usuario.Login == login);

        return result;
    }

    public async Task<string?> GetRoleNameAsync(int rolId)
    {
        var result = await _context.Set<Rol>()
            .Where(rol => rol.Id == rolId)
            .Select(rol => rol.Nombre)
            .FirstOrDefaultAsync();

        return result;
    }
    public async Task RegistrarAccesoAsync(
    int usuarioId,
    DateTime fechaAccesoUtc,
    string? nuevoPasswordHash)
    {
        var usuario = await _context.Set<Usuario>()
            .FirstOrDefaultAsync(x => x.Id == usuarioId);

        if (usuario is null)
            throw new InvalidOperationException(
                "No se encontró el usuario al registrar su acceso.");

        usuario.UltimoAccesoUtc = fechaAccesoUtc;

        if (nuevoPasswordHash is not null)
            usuario.PasswordHash = nuevoPasswordHash;

        await _context.SaveChangesAsync();
    }
}