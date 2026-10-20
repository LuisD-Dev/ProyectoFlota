namespace MantenimientoFlota.Application.Common.Exceptions;

public sealed class ResourceNotFoundException(string message) : BusinessException(message);
