using MantenimientoFlota.Domain.Entities;

namespace MantenimientoFlota.Application.Interfaces.Repositories;

public interface IProveedorRepository
{
    Task<IEnumerable<Proveedor>> GetAllAsync();

    Task<Proveedor?> GetByIdAsync(int id);

    Task<Proveedor> AddAsync(Proveedor proveedor);

    Task UpdateAsync(Proveedor proveedor);

    Task DeleteAsync(Proveedor proveedor);

    Task<bool> HasAssociatedPartsAsync(int proveedorId);
}
