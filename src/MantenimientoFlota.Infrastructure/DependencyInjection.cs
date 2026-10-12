using MantenimientoFlota.Application.Interfaces;
using MantenimientoFlota.Infrastructure.Implementation;
using MantenimientoFlota.Infrastructure.Persistence;
using MantenimientoFlota.Infrastructure.Persistence.Seed;
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

        services.AddDbContext<MantenimientoFlotaDbContext>(options =>
        {
            options.UseSqlServer(connectionString);

            if (enableDevelopmentSeeding)
            {
                options.UseSeeding((context, _) =>
                    DevelopmentSeeder.Seed((MantenimientoFlotaDbContext)context));

                options.UseAsyncSeeding((context, _, cancellationToken) =>
                    DevelopmentSeeder.SeedAsync(
                        (MantenimientoFlotaDbContext)context,
                        cancellationToken));
            }
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
