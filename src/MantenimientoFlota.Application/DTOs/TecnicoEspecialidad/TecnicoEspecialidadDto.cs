namespace MantenimientoFlota.Application.DTOs.TecnicoEspecialidad;

public sealed class TecnicoEspecialidadDto
{
    public int TecnicoId { get; set; }

    public string TecnicoNombreCompleto { get; set; } = string.Empty;

    public int EspecialidadId { get; set; }

    public string EspecialidadNombre { get; set; } = string.Empty;

    public string NivelCertificacion { get; set; } = string.Empty;

    public DateOnly FechaObtencion { get; set; }
}
