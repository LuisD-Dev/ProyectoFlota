using System.Security.Claims;
using System.Text;
using MantenimientoFlota.Application.Interfaces;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace MantenimientoFlota.Application.Implementation;

/// <summary>Genera tokens JWT firmados con HMAC-SHA256.</summary>
public class JwtTokenService : IJwtTokenService
{
    private readonly SigningCredentials _credentials;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expirationMinutes;

    public JwtTokenService(
        string key,
        string issuer,
        string audience,
        int expirationMinutes)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);

        if (keyBytes.Length < 32)
            throw new InvalidOperationException(
                "Jwt:Key debe tener al menos 32 bytes.");

        _credentials = new SigningCredentials(
            new SymmetricSecurityKey(keyBytes),
            SecurityAlgorithms.HmacSha256);

        _issuer = issuer;
        _audience = audience;
        _expirationMinutes = expirationMinutes;
    }

    public (string Token, DateTime ExpiresAt) CreateToken(
        int usuarioId,
        string login,
        string nombreCompleto,
        string rol)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_expirationMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuarioId.ToString()),
            new(JwtRegisteredClaimNames.Name, login),
            new("fullName", nombreCompleto),
            new("role", rol),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _issuer,
            Audience = _audience,
            Expires = expiresAt,
            SigningCredentials = _credentials
        };

        var token = new JsonWebTokenHandler().CreateToken(descriptor);

        return (token, expiresAt);
    }
}