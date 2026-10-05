using System.ComponentModel.DataAnnotations;

namespace MantenimientoFlota.Application.DTOs.Proveedor;

public sealed class ActualizarProveedorDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(200, ErrorMessage = "El contacto no puede superar los 200 caracteres.")]
    public string? Contacto { get; set; }

    [MaxLength(500, ErrorMessage = "La dirección no puede superar los 500 caracteres.")]
    public string? Direccion { get; set; }

    [MaxLength(
        1000,
        ErrorMessage = "Las condiciones generales no pueden superar los 1000 caracteres.")]
    public string? CondicionesGenerales { get; set; }
}
