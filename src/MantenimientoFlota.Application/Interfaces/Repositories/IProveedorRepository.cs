using MantenimientoFlota.Domain.Entities;
using MantenimientoFlota.Application.DTOs.Proveedor;

namespace MantenimientoFlota.Application.Interfaces.Repositories;

public interface IProveedorRepository
{
    Task<IEnumerable<Proveedor>> GetAllAsync();

    Task<Proveedor?> GetByIdAsync(int id);

    Task<ProveedorDetalleDto?> GetDetalleByIdAsync(int id);

    Task<Proveedor> AddAsync(Proveedor proveedor);

    Task UpdateAsync(Proveedor proveedor);

    Task DeleteAsync(Proveedor proveedor);

    Task<bool> HasAssociatedPartsAsync(int proveedorId);
}
