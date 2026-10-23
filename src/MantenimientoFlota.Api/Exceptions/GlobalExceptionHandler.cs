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
        if (exception is BadHttpRequestException badRequest)
        {
            logger.LogWarning("Solicitud HTTP rechazada. TraceId={TraceId} Status={Status} ExceptionType={ExceptionType}",
                context.TraceIdentifier, badRequest.StatusCode, badRequest.GetType().FullName);
            context.Response.StatusCode = badRequest.StatusCode;
            await context.Response.WriteAsJsonAsync(ApiErrorFactory.Create(context, badRequest.StatusCode,
                "La solicitud HTTP no es válida.", title: "Solicitud no válida", errorCode: "INVALID_REQUEST"), cancellationToken);
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
