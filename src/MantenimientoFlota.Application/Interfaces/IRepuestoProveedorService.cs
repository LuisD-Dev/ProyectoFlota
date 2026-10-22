using MantenimientoFlota.Domain.DTO.PatternResult;
using MantenimientoFlota.Domain.DTO.RepuestoProveedor;

namespace MantenimientoFlota.Application.Interfaces;

public interface IRepuestoProveedorService
{
    Task<Result<IReadOnlyList<RepuestoProveedorDto>>> GetAllAsync(int repuestoId);

    Task<Result<RepuestoProveedorDto>> GetByIdAsync(
        int repuestoId,
        int proveedorId);

    Task<Result<RepuestoProveedorDto>> CreateAsync(
        int repuestoId,
        CrearRepuestoProveedorDto dto);

    Task<Result<object?>> UpdateAsync(
        int repuestoId,
        int proveedorId,
        ActualizarRepuestoProveedorDto dto);

    Task<Result<object?>> DeleteAsync(
        int repuestoId,
        int proveedorId);
}
