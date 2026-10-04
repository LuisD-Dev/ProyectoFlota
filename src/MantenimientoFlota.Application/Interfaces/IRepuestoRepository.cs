using MantenimientoFlota.Domain.Entities;

namespace MantenimientoFlota.Application.Interfaces;

public interface IRepuestoRepository
{
    Task<IEnumerable<Repuesto>> GetAllAsync();

    Task<Repuesto?> GetByIdAsync(int id);

    Task<Repuesto?> AddAsync(Repuesto repuesto);

    void Update(Repuesto repuesto);

    void Delete(int id);

    Task<bool> HasAssociatedSuppliersAsync(int repuestoId);
}
