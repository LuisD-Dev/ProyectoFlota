using MantenimientoFlota.Domain.Constants;

namespace MantenimientoFlota.Domain.DTO.OrdenMantenimiento;

public sealed class OrdenMantenimientoDto
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

    [System.ComponentModel.Description("Estado de la orden: 0 Solicitada, 1 Diagnosticada, 2 Aprobada, 3 Rechazada, 4 EnEjecucion, 5 EnEsperaDeRepuesto, 6 Completada, 7 Cancelada. Al crear se asigna 0 en el servidor.")]
    public int EstadoActual { get; set; }
}
