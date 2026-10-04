using System.ComponentModel.DataAnnotations;

namespace MantenimientoFlota.Application.DTOs.TecnicoEspecialidad;

public sealed class ActualizarTecnicoEspecialidadDto
{
    [Required(ErrorMessage = "El nivel de certificación es obligatorio.")]
    [MaxLength(
        100,
        ErrorMessage = "El nivel de certificación no puede superar los 100 caracteres.")]
    public string NivelCertificacion { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de obtención es obligatoria.")]
    public DateOnly? FechaObtencion { get; set; }
}
