namespace MantenimientoFlota.Domain.Exceptions;

public abstract class BusinessException : Exception
{
    protected BusinessException(int status, string title, string message,
        Dictionary<string, string[]>? errors = null) : base(message)
    {
        Status = status;
        Title = title;
        Errors = errors ?? new Dictionary<string, string[]>();
    }

    public int Status { get; }
    public string Title { get; }
    public Dictionary<string, string[]> Errors { get; }
}
