using System.ComponentModel.DataAnnotations;

namespace MantenimientoFlota.Domain.DTO.Repuesto;

/// <summary>Contrato de crear repuesto.</summary>
public sealed class CrearRepuestoDto
{
    /// <summary>Código identificador del recurso.</summary>
    /// <example>REP-001</example>
    [Required(ErrorMessage = "El código es obligatorio.")]
    [MaxLength(50, ErrorMessage = "El código no puede superar los 50 caracteres.")]
    public string Codigo { get; set; } = string.Empty;

    /// <summary>Nombre.</summary>
    /// <example>Vehículo de servicio</example>
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Unidad utilizada para medir las existencias.</summary>
    /// <example>Unidad</example>
    [Required(ErrorMessage = "La unidad de medida es obligatoria.")]
    [MaxLength(50, ErrorMessage = "La unidad de medida no puede superar los 50 caracteres.")]
    public string UnidadMedida { get; set; } = string.Empty;

    /// <summary>Cantidad disponible del repuesto.</summary>
    /// <example>5</example>
    [Range(0, double.MaxValue, ErrorMessage = "El stock actual no puede ser negativo.")]
    public decimal StockActual { get; set; }

    /// <summary>Cantidad mínima requerida del repuesto.</summary>
    /// <example>10</example>
    [Range(0, double.MaxValue, ErrorMessage = "El stock mínimo no puede ser negativo.")]
    public decimal StockMinimo { get; set; }
}
