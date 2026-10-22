using MantenimientoFlota.Domain.Exceptions;
using MantenimientoFlota.Api.Common;
using Microsoft.AspNetCore.Diagnostics;

namespace MantenimientoFlota.Api.Exceptions;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception,
        CancellationToken cancellationToken)
    {
        if (context.Response.HasStarted) return false;
        if (context.Features.Get<IExceptionHandlerFeature>()?.Endpoint is RouteEndpoint endpoint)
            context.Items[RequestLoggingState.RouteKey] = endpoint.RoutePattern.RawText;

        if (exception is BusinessException business)
        {
            logger.LogWarning("Excepción de negocio procesada. TraceId={TraceId} ExceptionType={ExceptionType}",
                context.TraceIdentifier, business.GetType().FullName);
            context.Response.StatusCode = business.Status;
            await context.Response.WriteAsJsonAsync(ApiErrorFactory.Create(context,
                business.Status, business.Message, business.Errors, business.Title), cancellationToken);
            return true;
        }
        var conflict = ExceptionClassification.IsPersistenceConflict(exception);
        var status = conflict ? StatusCodes.Status409Conflict : StatusCodes.Status500InternalServerError;

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
