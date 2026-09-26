using MantenimientoFlota.Application.Interfaces;
using MantenimientoFlota.Infrastructure.Implementation;
using MantenimientoFlota.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MantenimientoFlota.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "La cadena de conexión 'DefaultConnection' no está configurada.");

        services.AddDbContext<MantenimientoFlotaDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<ICategoriaActivoRepository, CategoriaActivoRepository>();

        return services;
    }
}
