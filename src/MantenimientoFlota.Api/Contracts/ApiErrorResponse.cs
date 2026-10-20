using System.ComponentModel;
using System.Text.Json.Serialization;

namespace MantenimientoFlota.Api.Contracts;

public sealed record ApiErrorResponse(
    int Status,
    string Title,
    string Detail,
    string TraceId,
    Dictionary<string, string[]> Errors,
    [property: JsonPropertyName("errorCode")]
    [property: Description("Código estable del error, independiente del estado HTTP: VALIDATION_ERROR, INVALID_REQUEST, RESOURCE_NOT_FOUND, CONFLICT o UNEXPECTED_ERROR.")]
    string ErrorCode,
    [property: JsonPropertyName("timestampUtc")]
    [property: Description("Instante de construcción de la respuesta en UTC, con formato ISO 8601 y offset cero (+00:00).")]
    DateTimeOffset TimestampUtc)
{
    public bool IsSuccess => false;
    public object? Data => null;
}
