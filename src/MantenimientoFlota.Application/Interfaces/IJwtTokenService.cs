namespace MantenimientoFlota.Application.Interfaces;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAt) CreateToken(
        int usuarioId,
        string login,
        string nombreCompleto,
        string rol);
}