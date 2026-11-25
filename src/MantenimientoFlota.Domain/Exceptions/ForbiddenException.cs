namespace MantenimientoFlota.Domain.Exceptions;

/// <summary>El usuario está autenticado, pero no puede acceder a este recurso concreto.</summary>
public sealed class ForbiddenException(string message,
    Dictionary<string, string[]>? errors = null)
    : BusinessException(403, "Acceso denegado", message, errors);
