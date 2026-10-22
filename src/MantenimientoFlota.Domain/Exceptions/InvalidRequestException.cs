namespace MantenimientoFlota.Domain.Exceptions;

public sealed class InvalidRequestException(string message,
    Dictionary<string, string[]>? errors = null)
    : BusinessException(400, "Solicitud no válida", message, errors);
