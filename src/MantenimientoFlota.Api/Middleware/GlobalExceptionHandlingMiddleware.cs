using MantenimientoFlota.Api.Common;

namespace MantenimientoFlota.Api.Middleware;

public sealed class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            var responseStarted = context.Response.HasStarted;
            _logger.LogError(
                "Excepción inesperada. TraceId={TraceId} Method={Method} Route={Route} Status={Status} Outcome={Outcome} ExceptionType={ExceptionType} ResponseStarted={ResponseStarted}",
                context.TraceIdentifier,
                context.Request.Method,
                GetRouteTemplate(context),
                responseStarted ? context.Response.StatusCode : StatusCodes.Status500InternalServerError,
                RequestOutcome.UnexpectedException.ToString(),
                exception.GetType().FullName,
                responseStarted);

            if (responseStarted)
            {
                throw;
            }

            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await context.Response.WriteAsJsonAsync(ApiErrorFactory.Create(
                context,
                StatusCodes.Status500InternalServerError,
                "Ocurrió un error interno en el servidor."));
            return;
        }

        var status = context.Response.StatusCode;
        if (status is >= 200 and < 300)
        {
            _logger.LogInformation(
                "Solicitud completada. TraceId={TraceId} Method={Method} Route={Route} Status={Status} Outcome={Outcome}",
                context.TraceIdentifier,
                context.Request.Method,
                GetRouteTemplate(context),
                status,
                RequestOutcome.Success.ToString());
        }
        else if (context.Items.TryGetValue(RequestOutcomeMarker.Key, out var marker)
            && marker is RequestOutcome outcome
            && outcome is RequestOutcome.Validation or RequestOutcome.ExpectedFailure)
        {
            _logger.LogWarning(
                "Solicitud rechazada. TraceId={TraceId} Method={Method} Route={Route} Status={Status} Outcome={Outcome}",
                context.TraceIdentifier,
                context.Request.Method,
                GetRouteTemplate(context),
                status,
                outcome.ToString());
        }
    }

    private static string? GetRouteTemplate(HttpContext context) =>
        (context.GetEndpoint() as RouteEndpoint)?.RoutePattern.RawText;
}
