using MantenimientoFlota.Infrastructure.Interfaces;
using MantenimientoFlota.Infrastructure.Implementation;
using MantenimientoFlota.Infrastructure.Data;
using MantenimientoFlota.Infrastructure.Data.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MantenimientoFlota.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        bool enableDevelopmentSeeding)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "La cadena de conexión 'DefaultConnection' no está configurada.");

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(connectionString);

            // EF ejecuta ambos callbacks bajo su bloqueo de inicialización/migraciones.
            options.UseSeeding((context, _) =>
            {
                var db = (AppDbContext)context;
                MasterSeeder.Seed(db, configuration["Seed:Admin:Password"]);
                if (enableDevelopmentSeeding) DevelopmentSeeder.Seed(db);
            });
            options.UseAsyncSeeding(async (context, _, cancellationToken) =>
            {
                var db = (AppDbContext)context;
                await MasterSeeder.SeedAsync(db, configuration["Seed:Admin:Password"], cancellationToken);
                if (enableDevelopmentSeeding) await DevelopmentSeeder.SeedAsync(db, cancellationToken);
            });
        });

        services.AddScoped<IActivoRepository, ActivoRepository>();
        services.AddScoped<ICategoriaActivoRepository, CategoriaActivoRepository>();
        services.AddScoped<IEspecialidadRepository, EspecialidadRepository>();
        services.AddScoped<ITecnicoRepository, TecnicoRepository>();
        services.AddScoped<
            ITecnicoEspecialidadRepository,
            TecnicoEspecialidadRepository>();
        services.AddScoped<IRepuestoRepository, RepuestoRepository>();
        services.AddScoped<IProveedorRepository, ProveedorRepository>();
        services.AddScoped<
            IRepuestoProveedorRepository,
            RepuestoProveedorRepository>();
        services.AddScoped<
            IOrdenMantenimientoRepository,
            OrdenMantenimientoRepository>();

        return services;
    }
}
