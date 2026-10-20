using MantenimientoFlota.Application.DTOs.Proveedor;

namespace MantenimientoFlota.Application.Interfaces.Services;

public interface IProveedorService
{
    Task<IReadOnlyList<ProveedorDto>> GetAllAsync();

    Task<ProveedorDetalleDto> GetByIdAsync(int id);

    Task<ProveedorDto> CreateAsync(CrearProveedorDto dto);

    Task UpdateAsync(
        int id,
        ActualizarProveedorDto dto);

    Task DeleteAsync(int id);
}
