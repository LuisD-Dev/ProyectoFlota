using MantenimientoFlota.Domain.Enums;

namespace MantenimientoFlota.Application.DTOs.OrdenMantenimiento;

public sealed class OrdenMantenimientoDetalleDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public int ActivoId { get; set; }
    public string ActivoCodigo { get; set; } = string.Empty;
    public string ActivoNombre { get; set; } = string.Empty;
    public string Motivo { get; set; } = string.Empty;
    public string TipoMantenimiento { get; set; } = string.Empty;
    public DateTime FechaSolicitud { get; set; }
    public DateTime? FechaProgramada { get; set; }
    public DateTime? FechaCierre { get; set; }
    public string Prioridad { get; set; } = string.Empty;
    public decimal CostoTotal { get; set; }
    public EstadoOrdenMantenimiento EstadoActual { get; set; }
    public ActivoOrdenResumenDto Activo { get; set; } = new();
    public int DiasTranscurridosDesdeSolicitud { get; set; }
}
