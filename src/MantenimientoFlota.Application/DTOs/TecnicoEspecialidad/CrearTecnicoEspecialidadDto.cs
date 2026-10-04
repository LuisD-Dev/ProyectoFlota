using System.ComponentModel.DataAnnotations;

namespace MantenimientoFlota.Application.DTOs.TecnicoEspecialidad;

public sealed class CrearTecnicoEspecialidadDto
{
    [Required(ErrorMessage = "El técnico es obligatorio.")]
    public int? TecnicoId { get; set; }

    [Required(ErrorMessage = "La especialidad es obligatoria.")]
    public int? EspecialidadId { get; set; }

    [Required(ErrorMessage = "El nivel de certificación es obligatorio.")]
    [MaxLength(
        100,
        ErrorMessage = "El nivel de certificación no puede superar los 100 caracteres.")]
    public string NivelCertificacion { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de obtención es obligatoria.")]
    public DateOnly? FechaObtencion { get; set; }
}
