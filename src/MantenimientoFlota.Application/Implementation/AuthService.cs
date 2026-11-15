using MantenimientoFlota.Application.Interfaces;
using MantenimientoFlota.Domain.DTO;
using MantenimientoFlota.Domain.DTO.PatternResult;
using MantenimientoFlota.Domain.Entities;
using MantenimientoFlota.Infrastructure.Interfaces;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace MantenimientoFlota.Application.Implementation;

public class AuthService : IAuthService
{
    private static readonly string DummyHash =
        BCrypt.Net.BCrypt.HashPassword(
            "usuario-inexistente",
            workFactor: 12);

    private readonly IUsuarioRepository _repository;
    private readonly IJwtTokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUsuarioRepository repository,
        IJwtTokenService tokenService,
        IMapper mapper,
        ILogger<AuthService> logger)
    {
        _repository = repository;
        _tokenService = tokenService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<LoginResponseDto>> LoginAsync(
        LoginRequestDto login)
    {
        var userLogin = login.Login!.Trim();
        var usuario = await _repository.GetByLoginAsync(userLogin);

        var (passwordValido, requiereConversion) =
            VerificarPassword(usuario, login.Password!);

        if (usuario is null || !passwordValido || !usuario.Activo)
        {
            _logger.LogWarning("Intento de login rechazado.");

            return Result<LoginResponseDto>.Failure(
                401,
                "Unauthorized",
                "Usuario o contraseña incorrectos.");
        }

        var rol = await _repository.GetRoleNameAsync(usuario.RolId);

        if (string.IsNullOrWhiteSpace(rol))
        {
            _logger.LogWarning(
                "Login rechazado por rol no disponible. UsuarioId={UsuarioId}",
                usuario.Id);

            return Result<LoginResponseDto>.Failure(
                401,
                "Unauthorized",
                "Usuario o contraseña incorrectos.");
        }

        string? nuevoHash = requiereConversion
            ? BCrypt.Net.BCrypt.HashPassword(
                login.Password!,
                workFactor: 12)
            : null;

        await _repository.RegistrarAccesoAsync(
            usuario.Id,
            DateTime.UtcNow,
            nuevoHash);

        var (token, expiresAt) = _tokenService.CreateToken(
            usuario.Id,
            usuario.Login,
            usuario.NombreCompleto,
            rol);

        var data = _mapper.Map<LoginResponseDto>(usuario);
        data.Token = token;
        data.TokenType = "Bearer";
        data.ExpiresAt = expiresAt;
        data.Rol = rol;

        _logger.LogInformation(
            "Login correcto. UsuarioId={UsuarioId} Rol={Rol}",
            usuario.Id,
            rol);

        return Result<LoginResponseDto>.Success(
            data,
            200,
            "OK",
            "Inicio de sesión correcto.");
    }

    private (bool Valido, bool RequiereConversion) VerificarPassword(
        Usuario? usuario,
        string password)
    {
        if (usuario is null)
        {
            BCrypt.Net.BCrypt.Verify(password, DummyHash);
            return (false, false);
        }

        try
        {
            if (usuario.PasswordHash.StartsWith(
                "$2",
                StringComparison.Ordinal))
            {
                return (
                    BCrypt.Net.BCrypt.Verify(
                        password,
                        usuario.PasswordHash),
                    false);
            }

            // Compatibilidad con los usuarios del Avance 1.
            var resultado = new PasswordHasher<Usuario>()
                .VerifyHashedPassword(
                    usuario,
                    usuario.PasswordHash,
                    password);

            var valido = resultado != PasswordVerificationResult.Failed;

            return (valido, valido);
        }
        catch (Exception exception) when (
            exception is BCrypt.Net.SaltParseException
            or FormatException
            or ArgumentException)
        {
            _logger.LogWarning(
                "Formato de hash no válido. UsuarioId={UsuarioId}",
                usuario.Id);

            return (false, false);
        }
    }
}