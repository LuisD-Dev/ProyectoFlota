using MantenimientoFlota.Domain.Entities;

namespace MantenimientoFlota.Infrastructure.Interfaces;

public interface IOrdenMantenimientoRepository
{
    Task<IReadOnlyList<OrdenMantenimiento>> GetAllAsync(int? activoId = null);

    Task<OrdenMantenimiento?> GetByIdAsync(int id);

    Task<OrdenMantenimiento?> GetDetalleByIdAsync(int id);

    Task<OrdenMantenimiento> AddAsync(
        OrdenMantenimiento ordenMantenimiento);

    Task<bool> ActivoExistsAsync(int activoId);
}
