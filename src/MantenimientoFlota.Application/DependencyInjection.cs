using MantenimientoFlota.Application.Implementation;
using MantenimientoFlota.Application.Interfaces.Services;
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
        services.AddScoped<IProveedorService, ProveedorService>();
        services.AddScoped<IRepuestoProveedorService, RepuestoProveedorService>();
        services.AddScoped<IOrdenMantenimientoService, OrdenMantenimientoService>();

        return services;
    }
}
