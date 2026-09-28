using MantenimientoFlota.Application.Implementation;
using MantenimientoFlota.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MantenimientoFlota.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IActivoService, ActivoService>();
        services.AddScoped<ICategoriaActivoService, CategoriaActivoService>();

        return services;
    }
}
