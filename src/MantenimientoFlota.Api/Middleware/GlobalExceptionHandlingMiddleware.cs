using System.Diagnostics;
using MantenimientoFlota.Api.Common;
using MantenimientoFlota.Application.Common.Exceptions;

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
        var stopwatch = Stopwatch.StartNew();
        Exception? unexpectedException = null;
        var businessFailure = false;
        var responseStarted = false;
        _logger.LogInformation(
            "Solicitud iniciada. TraceId={TraceId} Method={Method} Route={Route} Status={Status} DurationMs={DurationMs}",
            context.TraceIdentifier,
            context.Request.Method,
            GetRouteTemplate(context),
            context.Response.StatusCode,
            stopwatch.Elapsed.TotalMilliseconds);

        try
        {
            await _next(context);
        }
        catch (BusinessException exception) when (exception is InvalidRequestException
            or ResourceNotFoundException or ConflictException)
        {
            businessFailure = true;
            responseStarted = context.Response.HasStarted;
            if (responseStarted)
            {
                throw;
            }

            var status = exception switch
            {
                InvalidRequestException => StatusCodes.Status400BadRequest,
                ResourceNotFoundException => StatusCodes.Status404NotFound,
                ConflictException => StatusCodes.Status409Conflict,
                _ => throw new InvalidOperationException()
            };
            context.Response.Clear();
            context.Response.StatusCode = status;
            await context.Response.WriteAsJsonAsync(ApiErrorFactory.Create(context, status, exception.Message));
        }
        catch (Exception exception)
        {
            unexpectedException = exception;
            responseStarted = context.Response.HasStarted;

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
        }
        finally
        {
            stopwatch.Stop();
            var status = context.Response.StatusCode;
            if (unexpectedException is not null)
            {
                _logger.LogError(
                    "Excepción inesperada. TraceId={TraceId} Method={Method} Route={Route} Status={Status} DurationMs={DurationMs} Outcome={Outcome} ExceptionType={ExceptionType} StackTrace={StackTrace} ResponseStarted={ResponseStarted}",
                    context.TraceIdentifier,
                    context.Request.Method,
                    GetRouteTemplate(context),
                    status,
                    stopwatch.Elapsed.TotalMilliseconds,
                    RequestOutcome.UnexpectedException.ToString(),
                    unexpectedException.GetType().FullName,
                    unexpectedException.StackTrace,
                    responseStarted);
            }
            else if (businessFailure)
            {
                _logger.LogWarning(
                    "Solicitud rechazada. TraceId={TraceId} Method={Method} Route={Route} Status={Status} DurationMs={DurationMs} Outcome={Outcome} ResponseStarted={ResponseStarted}",
                    context.TraceIdentifier,
                    context.Request.Method,
                    GetRouteTemplate(context),
                    status,
                    stopwatch.Elapsed.TotalMilliseconds,
                    RequestOutcome.ExpectedFailure.ToString(),
                    responseStarted);
            }
            else if (status is >= 200 and < 300)
            {
                _logger.LogInformation(
                    "Solicitud completada. TraceId={TraceId} Method={Method} Route={Route} Status={Status} DurationMs={DurationMs} Outcome={Outcome}",
                    context.TraceIdentifier,
                    context.Request.Method,
                    GetRouteTemplate(context),
                    status,
                    stopwatch.Elapsed.TotalMilliseconds,
                    RequestOutcome.Success.ToString());
            }
            else if (context.Items.TryGetValue(RequestOutcomeMarker.Key, out var marker)
                && marker is RequestOutcome outcome
                && outcome is RequestOutcome.Validation or RequestOutcome.ExpectedFailure)
            {
                _logger.LogWarning(
                    "Solicitud rechazada. TraceId={TraceId} Method={Method} Route={Route} Status={Status} DurationMs={DurationMs} Outcome={Outcome}",
                    context.TraceIdentifier,
                    context.Request.Method,
                    GetRouteTemplate(context),
                    status,
                    stopwatch.Elapsed.TotalMilliseconds,
                    outcome.ToString());
            }
            else
            {
                // Registrar el cierre sin atribuir una clasificación de negocio no establecida.
                _logger.LogInformation(
                    "Solicitud finalizada. TraceId={TraceId} Method={Method} Route={Route} Status={Status} DurationMs={DurationMs}",
                    context.TraceIdentifier,
                    context.Request.Method,
                    GetRouteTemplate(context),
                    status,
                    stopwatch.Elapsed.TotalMilliseconds);
            }
        }
    }

    private static string? GetRouteTemplate(HttpContext context) =>
        (context.GetEndpoint() as RouteEndpoint)?.RoutePattern.RawText;
}
