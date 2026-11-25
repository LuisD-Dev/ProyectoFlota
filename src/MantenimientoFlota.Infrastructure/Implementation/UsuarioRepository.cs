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

    public async Task<IReadOnlyList<Usuario>> GetAllAsync() =>
        await _context.Usuarios.AsNoTracking().Include(x => x.Rol).OrderBy(x => x.Login).ToListAsync();

    public Task<Usuario?> GetByIdAsync(int id) =>
        _context.Usuarios.AsNoTracking().Include(x => x.Rol).SingleOrDefaultAsync(x => x.Id == id);

    public async Task<IReadOnlyList<Rol>> GetRolesAsync() =>
        await _context.Roles.AsNoTracking().OrderBy(x => x.Id).ToListAsync();

    public async Task<Usuario> CreateAsync(Usuario usuario)
    {
        await ValidarUnicosAsync(usuario);
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        return usuario;
    }

    private async Task ValidarUnicosAsync(Usuario usuario)
    {
        if (await _context.Usuarios.AnyAsync(x => x.Id != usuario.Id &&
            (x.Login == usuario.Login || x.Correo == usuario.Correo)))
            throw new MantenimientoFlota.Domain.Exceptions.ConflictException("El login o correo ya está registrado.");
    }

    // Serializa las modificaciones de administradores para no dejar el sistema sin acceso.
    public async Task UpdateAsync(Usuario usuario)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        var actual = await _context.Usuarios.SingleOrDefaultAsync(x => x.Id == usuario.Id)
            ?? throw new MantenimientoFlota.Domain.Exceptions.ResourceNotFoundException("No se encontró el usuario.");
        await ProtegerAdministradorAsync(actual, usuario.RolId, actual.Activo);
        await ValidarUnicosAsync(usuario);
        actual.Login = usuario.Login;
        actual.NombreCompleto = usuario.NombreCompleto;
        actual.Correo = usuario.Correo;
        actual.RolId = usuario.RolId;
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    public async Task DesactivarAsync(int id)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        var actual = await _context.Usuarios.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new MantenimientoFlota.Domain.Exceptions.ResourceNotFoundException("No se encontró el usuario.");
        await ProtegerAdministradorAsync(actual, actual.RolId, false);
        actual.Activo = false;
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    private async Task ProtegerAdministradorAsync(Usuario actual, int rolNuevo, bool activoNuevo)
    {
        var admin = MantenimientoFlota.Infrastructure.Data.Seed.SeedIds.RolAdministrador;
        if (actual.Activo && actual.RolId == admin && (!activoNuevo || rolNuevo != admin) &&
            !await _context.Usuarios.AnyAsync(x => x.Id != actual.Id && x.Activo && x.RolId == admin))
            throw new MantenimientoFlota.Domain.Exceptions.ConflictException("Debe conservar al menos un administrador activo.");
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
