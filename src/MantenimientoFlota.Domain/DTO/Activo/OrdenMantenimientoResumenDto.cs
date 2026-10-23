using MantenimientoFlota.Domain.Constants;

namespace MantenimientoFlota.Domain.DTO.Activo;

/// <summary>Contrato de orden mantenimiento resumen.</summary>
public sealed class OrdenMantenimientoResumenDto
{
    /// <summary>Código identificador del recurso.</summary>
    /// <example>OM-DEMO-001</example>
    public string Codigo { get; set; } = string.Empty;
    /// <summary>Fecha y hora en que se registró la solicitud.</summary>
    /// <example>2026-10-20T08:00:00Z</example>
    public DateTime FechaSolicitud { get; set; }
    /// <summary>Tipo de mantenimiento solicitado.</summary>
    /// <example>Preventivo</example>
    public string TipoMantenimiento { get; set; } = string.Empty;
    /// <summary>Prioridad.</summary>
    /// <example>Alta</example>
    public string Prioridad { get; set; } = string.Empty;
    /// <summary>Estado actual de la orden; al crear se asigna Solicitada (0).</summary>
    /// <example>0</example>
    [System.ComponentModel.Description("Estado de la orden: 0 Solicitada, 1 Diagnosticada, 2 Aprobada, 3 Rechazada, 4 EnEjecucion, 5 EnEsperaDeRepuesto, 6 Completada, 7 Cancelada. Al crear se asigna 0 en el servidor.")]
    public int EstadoActual { get; set; }
}
