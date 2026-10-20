using MantenimientoFlota.Domain.Enums;

namespace MantenimientoFlota.Application.DTOs.Activo;

public sealed class OrdenMantenimientoResumenDto
{
    public string Codigo { get; set; } = string.Empty;
    public DateTime FechaSolicitud { get; set; }
    public string TipoMantenimiento { get; set; } = string.Empty;
    public string Prioridad { get; set; } = string.Empty;
    public EstadoOrdenMantenimiento EstadoActual { get; set; }
}
