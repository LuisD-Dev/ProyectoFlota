using MantenimientoFlota.Domain.Entities;

namespace MantenimientoFlota.Application.Interfaces;

public interface IRepuestoProveedorRepository
{
    Task<IReadOnlyList<RepuestoProveedor>> GetAllAsync();

    Task<RepuestoProveedor?> GetByIdAsync(
        int repuestoId,
        int proveedorId);

    Task<RepuestoProveedor> AddAsync(
        RepuestoProveedor repuestoProveedor);

    Task UpdateAsync(RepuestoProveedor repuestoProveedor);

    Task DeleteAsync(int repuestoId, int proveedorId);

    Task<bool> ExistsAsync(int repuestoId, int proveedorId);

    Task<bool> RepuestoExistsAsync(int repuestoId);

    Task<bool> ProveedorExistsAsync(int proveedorId);
}
