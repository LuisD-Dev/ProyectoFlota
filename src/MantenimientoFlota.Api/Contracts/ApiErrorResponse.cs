using System.ComponentModel;
using MantenimientoFlota.Domain.DTO.PatternResult;

namespace MantenimientoFlota.Api.Contracts;

public sealed class ApiErrorResponse : Result<object?>
{
    public string TraceId { get; }
    public Dictionary<string, string[]> Errors { get; }
    [Description("Código estable del error, independiente del estado HTTP.")]
    public string ErrorCode { get; }
    [Description("Instante UTC de construcción de la respuesta, en ISO 8601.")]
    public DateTimeOffset TimestampUtc { get; }

    public ApiErrorResponse(int status, string title, string detail, string traceId,
        Dictionary<string, string[]> errors, string errorCode, DateTimeOffset timestampUtc)
    {
        IsSuccess = false;
        Status = status;
        Title = title;
        Detail = detail;
        TraceId = traceId;
        Errors = errors;
        ErrorCode = errorCode;
        TimestampUtc = timestampUtc;
    }
}
