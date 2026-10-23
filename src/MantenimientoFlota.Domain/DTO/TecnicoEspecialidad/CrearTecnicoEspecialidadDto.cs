using System.ComponentModel.DataAnnotations;

namespace MantenimientoFlota.Domain.DTO.TecnicoEspecialidad;

/// <summary>Contrato de crear tecnico especialidad.</summary>
public sealed class CrearTecnicoEspecialidadDto
{
    /// <summary>Identificador del recurso asociado; debe corresponder a un registro existente.</summary>
    /// <example>1</example>
    [Required(ErrorMessage = "La especialidad es obligatoria.")]
    [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")]
    public int? EspecialidadId { get; set; }

    /// <summary>Nivel de certificación del técnico en la especialidad.</summary>
    /// <example>Avanzado</example>
    [Required(ErrorMessage = "El nivel de certificación es obligatorio.")]
    [MaxLength(
        100,
        ErrorMessage = "El nivel de certificación no puede superar los 100 caracteres.")]
    public string NivelCertificacion { get; set; } = string.Empty;

    /// <summary>Fecha de obtención de la certificación.</summary>
    /// <example>2020-01-15</example>
    [Required(ErrorMessage = "La fecha de obtención es obligatoria.")]
    public DateOnly? FechaObtencion { get; set; }
}
