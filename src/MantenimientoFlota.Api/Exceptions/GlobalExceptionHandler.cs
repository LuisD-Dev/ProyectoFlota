using MantenimientoFlota.Api.Common;
using Microsoft.AspNetCore.Diagnostics;

namespace MantenimientoFlota.Api.Exceptions;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception,
        CancellationToken cancellationToken)
    {
        if (context.Response.HasStarted) return false;
        var conflict = ExceptionClassification.IsPersistenceConflict(exception);
        var status = conflict ? StatusCodes.Status409Conflict : StatusCodes.Status500InternalServerError;
        if (context.Features.Get<IExceptionHandlerFeature>()?.Endpoint is RouteEndpoint endpoint)
            context.Items[RequestLoggingState.RouteKey] = endpoint.RoutePattern.RawText;

        // No se registra el objeto original: sus mensajes/inner exceptions pueden contener secretos.
        logger.Log(conflict ? LogLevel.Warning : LogLevel.Error,
            "Excepción procesada. TraceId={TraceId} ExceptionType={ExceptionType} StackTrace={StackTrace}",
            context.TraceIdentifier, exception.GetType().FullName, exception.StackTrace);
        context.Response.StatusCode = status;
        var response = ApiErrorFactory.Create(context, status,
            conflict ? "La operación entra en conflicto con los datos actuales."
                : "Ocurrió un error interno en el servidor.");
        await context.Response.WriteAsJsonAsync(response, cancellationToken);
        return true;
    }
}
