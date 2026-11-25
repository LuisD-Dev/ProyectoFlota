using MantenimientoFlota.Api.Common;
using MantenimientoFlota.Api.Contracts;
using MantenimientoFlota.Application.Interfaces;
using MantenimientoFlota.Domain.DTO;
using MantenimientoFlota.Domain.DTO.PatternResult;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace MantenimientoFlota.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    /// <summary>Iniciar sesión y obtener un token JWT.</summary>
    [Microsoft.AspNetCore.RateLimiting.EnableRateLimiting("login")]
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Result<LoginResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiErrorResponse), 400)]
    [ProducesResponseType(typeof(ApiErrorResponse), 401)]
    [ProducesResponseType(typeof(ApiErrorResponse), 500)]
    public async Task<IActionResult> LoginAsync(
        [FromBody] LoginRequestDto login)
    {
        var result = await _service.LoginAsync(login);

        return this.FromResult(result);
    }

    /// <summary>Consultar los datos del usuario autenticado.</summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(Result<CurrentUserDto>), 200)]
    [ProducesResponseType(401)]
    public IActionResult Me()
    {
        var data = new CurrentUserDto
        {
            Id = User.FindFirst("sub")?.Value,
            Login = User.FindFirst("name")?.Value,
            NombreCompleto = User.FindFirst("fullName")?.Value,
            Rol = User.FindFirst("role")?.Value
        };

        return Ok(Result<CurrentUserDto>.Success(data));
    }
}
