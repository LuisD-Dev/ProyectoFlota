using MantenimientoFlota.Application.Common.Results;
using MantenimientoFlota.Application.DTOs.RepuestoProveedor;

namespace MantenimientoFlota.Application.Interfaces.Services;

public interface IRepuestoProveedorService
{
    Task<IReadOnlyList<RepuestoProveedorDto>> GetAllAsync();

    Task<RepuestoProveedorDto?> GetByIdAsync(
        int repuestoId,
        int proveedorId);

    Task<RepuestoProveedorOperationResult> CreateAsync(
        CrearRepuestoProveedorDto dto);

    Task<RepuestoProveedorOperationResult> UpdateAsync(
        int repuestoId,
        int proveedorId,
        ActualizarRepuestoProveedorDto dto);

    Task<RepuestoProveedorOperationResult> DeleteAsync(
        int repuestoId,
        int proveedorId);
}
