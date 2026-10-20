namespace MantenimientoFlota.Application.DTOs.Especialidad;

public sealed class TecnicoEspecialidadResumenDto
{
    public int TecnicoId { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string NivelCertificacion { get; set; } = string.Empty;
    public bool Disponible { get; set; }
}
