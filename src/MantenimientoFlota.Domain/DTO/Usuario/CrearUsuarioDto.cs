using System.ComponentModel.DataAnnotations;
namespace MantenimientoFlota.Domain.DTO.Usuario;

public sealed class CrearUsuarioDto
{
    [Required, MaxLength(50), RegularExpression(@"^[a-zA-Z0-9._@-]+$")]
    public string Login { get; set; } = string.Empty;
    [Required, MaxLength(150), RegularExpression(@"^[^<>]*$")]
    public string NombreCompleto { get; set; } = string.Empty;
    [Required, EmailAddress, MaxLength(150)]
    public string Correo { get; set; } = string.Empty;
    [Range(1, int.MaxValue)]
    public int RolId { get; set; }
    [Required, MinLength(12), MaxLength(72)]
    public string Password { get; set; } = string.Empty;
}
