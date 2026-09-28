using System.ComponentModel.DataAnnotations;

namespace MantenimientoFlota.Application.DTOs.CategoriaActivo;

public sealed class ActualizarCategoriaActivoDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
    public string? Descripcion { get; set; }

    public int IntervaloMantenimientoKm { get; set; }
}
