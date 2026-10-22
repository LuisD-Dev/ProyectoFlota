using Serilog;
using Serilog.Events;

namespace MantenimientoFlota.Api.Common;

public static class ApiLoggingConfiguration
{
    public static LoggerConfiguration ConfigureApiLogging(this LoggerConfiguration configuration,
        string logDirectory, bool writeConsole = true)
    {
        const string template = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}";
        configuration.MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Information)
            .Enrich.FromLogContext()
            // Los mensajes de EF Update pueden incluir la excepción completa como propiedad de texto.
            // Los fallos HTTP se diagnostican de forma segura en nuestro handler/cierre.
            .Filter.ByExcluding(logEvent =>
                logEvent.Properties.TryGetValue("SourceContext", out var source)
                && source is ScalarValue { Value: string category }
                && ((category.StartsWith("Microsoft.", StringComparison.Ordinal) && logEvent.Exception is not null)
                    || category == "Microsoft.EntityFrameworkCore.Update" && logEvent.Level >= LogEventLevel.Error));
        if (writeConsole) configuration.WriteTo.Console(outputTemplate: template);
        return configuration.WriteTo.File(Path.Combine(logDirectory, "serilog-.txt"),
            rollingInterval: RollingInterval.Day, retainedFileCountLimit: 14,
            fileSizeLimitBytes: 10_485_760, rollOnFileSizeLimit: true, shared: true,
            outputTemplate: template);
    }

    public static IApplicationBuilder UseApiRequestLogging(this IApplicationBuilder app)
    {
        return app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate = "Solicitud finalizada. TraceId={TraceId} Method={RequestMethod} Route={Route} Status={StatusCode} DurationMs={Elapsed:0.0000} Outcome={Outcome}";
            options.GetLevel = (context, _, exception) =>
            {
                if (context.Items.ContainsKey(RequestLoggingState.UnhandledKey)) return LogEventLevel.Verbose;
                if (exception is not null || context.Response.StatusCode >= 500) return LogEventLevel.Error;
                return context.Items.ContainsKey(RequestOutcomeMarker.Key) ? LogEventLevel.Warning : LogEventLevel.Information;
            };
            options.EnrichDiagnosticContext = (diagnostic, context) =>
            {
                diagnostic.Set("TraceId", context.TraceIdentifier);
                diagnostic.Set("Route", RequestLoggingState.Route(context));
                diagnostic.Set("Outcome", context.Items.TryGetValue(RequestOutcomeMarker.Key, out var outcome)
                    ? outcome! : context.Response.StatusCode >= 500 ? RequestOutcome.UnexpectedException
                    : context.Response.StatusCode is >= 200 and < 300 ? RequestOutcome.Success : "Unclassified");
            };
        });
    }
}
