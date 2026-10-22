using System.Text.Json.Serialization;

namespace MantenimientoFlota.Domain.DTO.PatternResult;

public class Result<T> : BaseResult
{
    public T? Data { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? PageNumber { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? PageSize { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? TotalRecords { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? TotalPages { get; set; }

    public static Result<T> Paged(T data, int pageNumber, int pageSize, int totalRecords)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(pageNumber, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);
        ArgumentOutOfRangeException.ThrowIfNegative(totalRecords);
        return new()
        {
            IsSuccess = true, Status = 200, Title = "OK", Data = data,
            PageNumber = pageNumber, PageSize = pageSize, TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize)
        };
    }

    public static Result<T> Success(T? data, int status = 200, string title = "OK", string detail = "") =>
        new() { IsSuccess = true, Status = status, Title = title, Detail = detail, Data = data };

    public static Result<T> Failure(int status, string title, string detail) =>
        new() { IsSuccess = false, Status = status, Title = title, Detail = detail };
}
