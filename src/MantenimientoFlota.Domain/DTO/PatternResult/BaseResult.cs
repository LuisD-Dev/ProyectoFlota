namespace MantenimientoFlota.Domain.DTO.PatternResult;

public abstract class BaseResult
{
    public bool IsSuccess { get; set; }
    public int Status { get; set; }
    public string Title { get; set; } = "";
    public string Detail { get; set; } = "";
}
