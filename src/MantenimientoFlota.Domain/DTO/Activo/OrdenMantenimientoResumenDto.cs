using MantenimientoFlota.Domain.Constants;

namespace MantenimientoFlota.Domain.DTO.Activo;

public sealed class OrdenMantenimientoResumenDto
{
    public string Codigo { get; set; } = string.Empty;
    public DateTime FechaSolicitud { get; set; }
    public string TipoMantenimiento { get; set; } = string.Empty;
    public string Prioridad { get; set; } = string.Empty;
    [System.ComponentModel.Description("Estado de la orden: 0 Solicitada, 1 Diagnosticada, 2 Aprobada, 3 Rechazada, 4 EnEjecucion, 5 EnEsperaDeRepuesto, 6 Completada, 7 Cancelada. Al crear se asigna 0 en el servidor.")]
    public int EstadoActual { get; set; }
}
