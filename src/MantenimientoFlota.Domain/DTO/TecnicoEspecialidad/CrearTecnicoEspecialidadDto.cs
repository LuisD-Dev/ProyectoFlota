using System.ComponentModel.DataAnnotations;

namespace MantenimientoFlota.Domain.DTO.TecnicoEspecialidad;

public sealed class CrearTecnicoEspecialidadDto
{
    [Required(ErrorMessage = "La especialidad es obligatoria.")]
    [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")]
    public int? EspecialidadId { get; set; }

    [Required(ErrorMessage = "El nivel de certificación es obligatorio.")]
    [MaxLength(
        100,
        ErrorMessage = "El nivel de certificación no puede superar los 100 caracteres.")]
    public string NivelCertificacion { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de obtención es obligatoria.")]
    public DateOnly? FechaObtencion { get; set; }
}
