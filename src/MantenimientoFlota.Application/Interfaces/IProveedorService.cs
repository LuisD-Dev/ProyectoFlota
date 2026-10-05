using MantenimientoFlota.Application.Common.Results;
using MantenimientoFlota.Application.DTOs.Proveedor;

namespace MantenimientoFlota.Application.Interfaces;

public interface IProveedorService
{
    Task<IReadOnlyList<ProveedorDto>> GetAllAsync();

    Task<ProveedorDto?> GetByIdAsync(int id);

    Task<ProveedorOperationResult> CreateAsync(CrearProveedorDto dto);

    Task<ProveedorOperationResult> UpdateAsync(
        int id,
        ActualizarProveedorDto dto);

    Task<ProveedorOperationResult> DeleteAsync(int id);
}
