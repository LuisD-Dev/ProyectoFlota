using System.ComponentModel.DataAnnotations;

namespace MantenimientoFlota.Domain.DTO;

/// <summary>Credenciales para iniciar sesión.</summary>
public record class LoginRequestDto
{
    [Required(ErrorMessage = "El usuario es obligatorio.")]
    [StringLength(50,
        ErrorMessage = "El usuario no puede superar 50 caracteres.")]
    public string? Login { get; set; }

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(100,
        ErrorMessage = "La contraseña no puede superar 100 caracteres.")]
    public string? Password { get; set; }
}