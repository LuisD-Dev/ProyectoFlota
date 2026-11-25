using MantenimientoFlota.Application.Interfaces;

namespace MantenimientoFlota.Api.Authorization;

/// <summary>Lee el usuario autenticado desde los claims del token ("sub" y "role").</summary>
public sealed class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    private System.Security.Claims.ClaimsPrincipal? Principal => accessor.HttpContext?.User;

    public int? UsuarioId =>
        int.TryParse(Principal?.FindFirst("sub")?.Value, out var id) ? id : null;

    public string? Rol => Principal?.FindFirst("role")?.Value;
}
