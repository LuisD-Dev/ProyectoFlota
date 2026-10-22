namespace MantenimientoFlota.Api.Common;

internal static class RequestLoggingState
{
    internal static readonly object UnhandledKey = new();
    internal static readonly object RouteKey = new();

    internal static string? Route(HttpContext context) =>
        (context.GetEndpoint() as RouteEndpoint)?.RoutePattern.RawText
        ?? (context.Items.TryGetValue(RouteKey, out var route) ? route as string : null);
}
