using MantenimientoFlota.Application.DTOs.RepuestoProveedor;

namespace MantenimientoFlota.Application.Interfaces.Services;

public interface IRepuestoProveedorService
{
    Task<IReadOnlyList<RepuestoProveedorDto>> GetAllAsync();

    Task<RepuestoProveedorDto> GetByIdAsync(
        int repuestoId,
        int proveedorId);

    Task<RepuestoProveedorDto> CreateAsync(
        CrearRepuestoProveedorDto dto);

    Task UpdateAsync(
        int repuestoId,
        int proveedorId,
        ActualizarRepuestoProveedorDto dto);

    Task DeleteAsync(
        int repuestoId,
        int proveedorId);
}
