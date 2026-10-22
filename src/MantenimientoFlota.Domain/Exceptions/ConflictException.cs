namespace MantenimientoFlota.Domain.Exceptions;

public sealed class ConflictException(string message,
    Dictionary<string, string[]>? errors = null)
    : BusinessException(409, "Conflicto", message, errors);
