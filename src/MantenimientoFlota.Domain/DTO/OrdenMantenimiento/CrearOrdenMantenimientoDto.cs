using System.ComponentModel.DataAnnotations;

namespace MantenimientoFlota.Domain.DTO.OrdenMantenimiento;

/// <summary>Contrato de crear orden mantenimiento.</summary>
public sealed class CrearOrdenMantenimientoDto
{
    /// <summary>Identificador del recurso asociado; debe corresponder a un registro existente.</summary>
    /// <example>1</example>
    [Required(ErrorMessage = "El activo es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")]
    public int? ActivoId { get; set; }

    /// <summary>Motivo.</summary>
    /// <example>Revisión preventiva de la unidad</example>
    [Required(ErrorMessage = "El motivo es obligatorio.")]
    [MaxLength(
        1000,
        ErrorMessage = "El motivo no puede exceder los 1000 caracteres.")]
    public string Motivo { get; set; } = string.Empty;

    /// <summary>Tipo de mantenimiento solicitado.</summary>
    /// <example>Preventivo</example>
    [Required(ErrorMessage = "El tipo de mantenimiento es obligatorio.")]
    [MaxLength(
        100,
        ErrorMessage = "El tipo de mantenimiento no puede exceder los 100 caracteres.")]
    public string TipoMantenimiento { get; set; } = string.Empty;

    /// <summary>Prioridad.</summary>
    /// <example>Alta</example>
    [Required(ErrorMessage = "La prioridad es obligatoria.")]
    [MaxLength(
        50,
        ErrorMessage = "La prioridad no puede exceder los 50 caracteres.")]
    public string Prioridad { get; set; } = string.Empty;
}
