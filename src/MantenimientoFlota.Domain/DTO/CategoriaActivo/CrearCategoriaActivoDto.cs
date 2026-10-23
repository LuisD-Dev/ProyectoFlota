using System.ComponentModel.DataAnnotations;

namespace MantenimientoFlota.Domain.DTO.CategoriaActivo;

/// <summary>Contrato de crear categoria activo.</summary>
public sealed class CrearCategoriaActivoDto
{
    /// <summary>Nombre.</summary>
    /// <example>Vehículo de servicio</example>
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Descripción del recurso.</summary>
    /// <example>Unidad de mantenimiento</example>
    [MaxLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
    public string? Descripcion { get; set; }

    /// <summary>Intervalo de mantenimiento de la categoría, expresado en kilómetros.</summary>
    /// <example>5000</example>
    [Range(0, int.MaxValue, ErrorMessage = "El intervalo de mantenimiento debe ser mayor o igual que cero.")]
    public int IntervaloMantenimientoKm { get; set; }
}
