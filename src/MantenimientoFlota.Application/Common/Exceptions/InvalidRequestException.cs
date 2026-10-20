namespace MantenimientoFlota.Application.Common.Exceptions;

public sealed class InvalidRequestException(string message) : BusinessException(message);
