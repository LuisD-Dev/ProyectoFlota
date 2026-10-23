using MantenimientoFlota.Domain.Constants;

namespace MantenimientoFlota.Domain.DTO.OrdenMantenimiento;

/// <summary>Contrato de orden mantenimiento detalle.</summary>
public sealed class OrdenMantenimientoDetalleDto
{
    /// <summary>Identificador asignado al recurso.</summary>
    /// <example>1</example>
    public int Id { get; set; }
    /// <summary>Código identificador del recurso.</summary>
    /// <example>OM-DEMO-001</example>
    public string Codigo { get; set; } = string.Empty;
    /// <summary>Identificador del recurso asociado; debe corresponder a un registro existente.</summary>
    /// <example>1</example>
    public int ActivoId { get; set; }
    /// <summary>Código del activo asociado.</summary>
    public string ActivoCodigo { get; set; } = string.Empty;
    /// <summary>Nombre del activo asociado.</summary>
    public string ActivoNombre { get; set; } = string.Empty;
    /// <summary>Motivo.</summary>
    /// <example>Revisión preventiva de la unidad</example>
    public string Motivo { get; set; } = string.Empty;
    /// <summary>Tipo de mantenimiento solicitado.</summary>
    /// <example>Preventivo</example>
    public string TipoMantenimiento { get; set; } = string.Empty;
    /// <summary>Fecha y hora en que se registró la solicitud.</summary>
    /// <example>2026-10-20T08:00:00Z</example>
    public DateTime FechaSolicitud { get; set; }
    /// <summary>Fecha y hora programadas, si existen.</summary>
    /// <example>2026-10-21T08:00:00Z</example>
    public DateTime? FechaProgramada { get; set; }
    /// <summary>Fecha y hora de cierre, si existe.</summary>
    /// <example>2026-10-22T08:00:00Z</example>
    public DateTime? FechaCierre { get; set; }
    /// <summary>Prioridad.</summary>
    /// <example>Alta</example>
    public string Prioridad { get; set; } = string.Empty;
    /// <summary>Costo total calculado para la orden.</summary>
    public decimal CostoTotal { get; set; }
    /// <summary>Estado actual de la orden; al crear se asigna Solicitada (0).</summary>
    /// <example>0</example>
    [System.ComponentModel.Description("Estado de la orden: 0 Solicitada, 1 Diagnosticada, 2 Aprobada, 3 Rechazada, 4 EnEjecucion, 5 EnEsperaDeRepuesto, 6 Completada, 7 Cancelada. Al crear se asigna 0 en el servidor.")]
    public int EstadoActual { get; set; }
    /// <summary>Información del activo asociado a la orden.</summary>
    public ActivoOrdenResumenDto Activo { get; set; } = new();
    /// <summary>Días transcurridos desde la fecha de solicitud.</summary>
    public int DiasTranscurridosDesdeSolicitud { get; set; }
}
