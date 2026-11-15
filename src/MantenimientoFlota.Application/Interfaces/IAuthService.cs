using MantenimientoFlota.Domain.DTO;
using MantenimientoFlota.Domain.DTO.PatternResult;

namespace MantenimientoFlota.Application.Interfaces;

public interface IAuthService
{
    Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto login);
}