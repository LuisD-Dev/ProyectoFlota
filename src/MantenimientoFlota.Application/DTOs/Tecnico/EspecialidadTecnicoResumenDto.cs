namespace MantenimientoFlota.Application.DTOs.Tecnico;

public sealed class EspecialidadTecnicoResumenDto
{
    public int EspecialidadId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string NivelCertificacion { get; set; } = string.Empty;
    public DateOnly FechaObtencion { get; set; }
    public int AntiguedadAnios { get; set; }
}
