namespace MantenimientoFlota.Domain.Exceptions;

public sealed class ResourceNotFoundException(string message,
    Dictionary<string, string[]>? errors = null)
    : BusinessException(404, "Recurso no encontrado", message, errors);
