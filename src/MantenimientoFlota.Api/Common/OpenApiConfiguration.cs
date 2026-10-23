namespace MantenimientoFlota.Api.Common;

public static class OpenApiConfiguration
{
    public static IServiceCollection AddApiDocumentation(this IServiceCollection services)
    {
        services.AddOpenApi(options => options.AddDocumentTransformer((document, context, cancellationToken) =>
        {
            document.Info.Title = "Mantenimiento de Flota API";
            document.Info.Version = "v1";
            document.Info.Description = "API del Equipo 7 para gestionar activos, categorías, técnicos, especialidades, repuestos, proveedores y solicitudes de mantenimiento. Las respuestas JSON usan Result; los errores incluyen traceId y errores por campo.";
            return Task.CompletedTask;
        }));
        return services;
    }
}
