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
        services.AddScoped<IEspecialidadService, EspecialidadService>();
        services.AddScoped<ITecnicoService, TecnicoService>();
        services.AddScoped<ITecnicoEspecialidadService, TecnicoEspecialidadService>();
        services.AddScoped<IRepuestoService, RepuestoService>();

        return services;
    }
}
