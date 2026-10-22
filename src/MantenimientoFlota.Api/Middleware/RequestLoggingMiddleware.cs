using System.Diagnostics;
using MantenimientoFlota.Api.Common;

namespace MantenimientoFlota.Api.Middleware;

/// <summary>Inicio y captura de fallos que escapan al handler; Serilog registra el cierre normal.</summary>
public sealed class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        using var scope = logger.BeginScope(new Dictionary<string, object> { ["TraceId"] = context.TraceIdentifier });
        logger.LogInformation("Solicitud iniciada. TraceId={TraceId} Method={Method} Route={Route}",
            context.TraceIdentifier, context.Request.Method, RequestLoggingState.Route(context));
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            // Evita un segundo cierre de Serilog con la excepción original; conserva la respuesta y relanza.
            context.Items[RequestLoggingState.UnhandledKey] = true;
            logger.LogError(
                "Solicitud interrumpida. TraceId={TraceId} Method={Method} Route={Route} Status={Status} DurationMs={DurationMs} ExceptionType={ExceptionType} StackTrace={StackTrace} ResponseStarted={ResponseStarted}",
                context.TraceIdentifier, context.Request.Method, RequestLoggingState.Route(context),
                context.Response.StatusCode, stopwatch.Elapsed.TotalMilliseconds,
                exception.GetType().FullName, exception.StackTrace, context.Response.HasStarted);
            throw;
        }
    }
}
