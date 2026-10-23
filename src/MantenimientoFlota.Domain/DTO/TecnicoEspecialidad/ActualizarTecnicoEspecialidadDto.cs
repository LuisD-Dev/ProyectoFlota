using System.ComponentModel.DataAnnotations;

namespace MantenimientoFlota.Domain.DTO.TecnicoEspecialidad;

/// <summary>Contrato de actualizar tecnico especialidad.</summary>
public sealed class ActualizarTecnicoEspecialidadDto
{
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
