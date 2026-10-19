namespace MantenimientoFlota.Api.Contracts;

public sealed record ApiErrorResponse(
    int Status,
    string Title,
    string Detail,
    string TraceId,
    Dictionary<string, string[]> Errors)
{
    public bool IsSuccess => false;
    public object? Data => null;
}
