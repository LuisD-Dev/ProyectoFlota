namespace MantenimientoFlota.Domain.DTO;

/// <summary>Token y datos del usuario después de iniciar sesión.</summary>
public record class LoginResponseDto
{
    public string? Token { get; set; }
    public string? TokenType { get; set; }
    public DateTime ExpiresAt { get; set; }

    public int Id { get; set; }
    public string? Login { get; set; }
    public string? NombreCompleto { get; set; }
    public string? Correo { get; set; }
    public string? Rol { get; set; }
}