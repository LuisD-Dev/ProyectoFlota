using MantenimientoFlota.Domain.DTO.Repuesto;

namespace MantenimientoFlota.Application.Interfaces;

/// <summary>Contrato para publicar alertas de stock bajo.</summary>
public interface IStockBajoNotifier
{
    Task PublicarAsync(
        StockBajoDto alerta,
        CancellationToken cancellationToken = default);
}