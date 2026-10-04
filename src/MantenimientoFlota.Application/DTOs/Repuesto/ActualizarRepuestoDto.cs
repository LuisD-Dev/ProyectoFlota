using System.ComponentModel.DataAnnotations;

namespace MantenimientoFlota.Application.DTOs.Repuesto;

public sealed class ActualizarRepuestoDto
{
    [Required(ErrorMessage = "El código es obligatorio.")]
    [MaxLength(50, ErrorMessage = "El código no puede superar los 50 caracteres.")]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La unidad de medida es obligatoria.")]
    [MaxLength(50, ErrorMessage = "La unidad de medida no puede superar los 50 caracteres.")]
    public string UnidadMedida { get; set; } = string.Empty;

    [Range(0, double.MaxValue, ErrorMessage = "El stock actual no puede ser negativo.")]
    public decimal StockActual { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "El stock mínimo no puede ser negativo.")]
    public decimal StockMinimo { get; set; }
}
