namespace MantenimientoFlota.Application.Common.Exceptions;

public sealed class ConflictException(string message) : BusinessException(message);
