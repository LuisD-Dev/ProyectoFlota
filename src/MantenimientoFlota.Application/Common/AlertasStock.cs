using MantenimientoFlota.Domain.DTO.Repuesto;
using MantenimientoFlota.Domain.Entities;

namespace MantenimientoFlota.Application.Common;

/// <summary>Evalúa el paso de un repuesto a stock bajo mínimo.</summary>
public static class AlertasStock
{
    public static StockBajoDto? CrearSiCorresponde(
        Repuesto repuesto,
        bool estabaBajoMinimo,
        decimal stockAnterior)
    {
        var estaBajoMinimo =
            repuesto.StockActual < repuesto.StockMinimo;

        if (estabaBajoMinimo || !estaBajoMinimo)
        {
            return null;
        }

        return new StockBajoDto
        {
            EventoId = Guid.NewGuid(),
            RepuestoId = repuesto.Id,
            Codigo = repuesto.Codigo,
            Nombre = repuesto.Nombre,
            StockAnterior = stockAnterior,
            StockActual = repuesto.StockActual,
            StockMinimo = repuesto.StockMinimo,
            FechaUtc = DateTime.UtcNow
        };
    }
}