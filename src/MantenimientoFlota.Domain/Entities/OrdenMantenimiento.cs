using MantenimientoFlota.Domain.Constants;

namespace MantenimientoFlota.Domain.Entities;

public class OrdenMantenimiento
{
    public int Id { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public int ActivoId { get; set; }

    public Activo Activo { get; set; } = null!;

    public string Motivo { get; set; } = string.Empty;

    public string TipoMantenimiento { get; set; } = string.Empty;

    public DateTime FechaSolicitud { get; set; }

    public DateTime? FechaProgramada { get; set; }

    public DateTime? FechaCierre { get; set; }

    public string Prioridad { get; set; } = string.Empty;

    public decimal CostoTotal { get; set; }

    public int EstadoActual { get; set; } =
        EstadoOrdenMantenimiento.Solicitada;
}
