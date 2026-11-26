namespace MantenimientoFlota.Domain.DTO.Repuesto;

/// <summary>Información de una alerta de stock bajo mínimo.</summary>
public sealed class StockBajoDto
{
    public Guid EventoId { get; set; }

    public int RepuestoId { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public decimal StockAnterior { get; set; }

    public decimal StockActual { get; set; }

    public decimal StockMinimo { get; set; }

    public DateTime FechaUtc { get; set; }
}