namespace MantenimientoFlota.Application.Interfaces;

/// <summary>Usuario autenticado de la petición actual, leído del token JWT.</summary>
public interface ICurrentUser
{
    /// <summary>Valor del claim "sub"; nulo si no hay sesión o no es un entero válido.</summary>
    int? UsuarioId { get; }

    /// <summary>Valor del claim "role".</summary>
    string? Rol { get; }
}
