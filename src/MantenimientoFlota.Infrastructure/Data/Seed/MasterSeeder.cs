using MantenimientoFlota.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MantenimientoFlota.Infrastructure.Data.Seed;

/// <summary>Administrador inicial; no reinicia credenciales ni reactiva usuarios existentes.</summary>
public static class MasterSeeder
{
    public static void Seed(AppDbContext context, string? adminPassword)
    {
        if (context.Usuarios.AsNoTracking().Any(x => x.Login == SeedIds.AdminLogin)) return;
        var admin = CreateAdmin(adminPassword);
        context.Usuarios.Add(admin);
        context.SaveChanges();
    }

    public static async Task SeedAsync(AppDbContext context, string? adminPassword,
        CancellationToken cancellationToken = default)
    {
        if (await context.Usuarios.AsNoTracking().AnyAsync(x => x.Login == SeedIds.AdminLogin, cancellationToken)) return;
        var admin = CreateAdmin(adminPassword);
        context.Usuarios.Add(admin);
        await context.SaveChangesAsync(cancellationToken);
    }

    private static Usuario CreateAdmin(string? password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 12)
            throw new InvalidOperationException("Configure Seed:Admin:Password con al menos 12 caracteres para crear el administrador inicial.");
        var admin = new Usuario
        {
            RolId = SeedIds.RolAdministrador,
            Login = SeedIds.AdminLogin,
            NombreCompleto = "Administrador General",
            Correo = "admin@flota.example",
            Activo = true,
            FechaCreacionUtc = DateTime.UtcNow
        };
        admin.PasswordHash = new PasswordHasher<Usuario>().HashPassword(admin, password);
        return admin;
    }
}
