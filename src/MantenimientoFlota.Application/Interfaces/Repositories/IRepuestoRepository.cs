using MantenimientoFlota.Domain.Entities;

namespace MantenimientoFlota.Application.Interfaces.Repositories;

public interface IRepuestoRepository
{
    Task<IEnumerable<Repuesto>> GetAllAsync(bool stockBajo = false);

    Task<Repuesto?> GetByIdAsync(int id);

    Task<Repuesto?> AddAsync(Repuesto repuesto);

    void Update(Repuesto repuesto);

    void Delete(int id);

    Task<bool> HasAssociatedSuppliersAsync(int repuestoId);
}
