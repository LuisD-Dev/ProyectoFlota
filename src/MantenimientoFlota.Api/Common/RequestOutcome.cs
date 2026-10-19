namespace MantenimientoFlota.Api.Common;

internal enum RequestOutcome
{
    Success,
    Validation,
    ExpectedFailure,
    UnexpectedException
}

internal static class RequestOutcomeMarker
{
    internal static readonly object Key = new();
}
