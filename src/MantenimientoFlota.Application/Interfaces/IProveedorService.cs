using MantenimientoFlota.Domain.DTO.PatternResult;
using MantenimientoFlota.Domain.DTO.Proveedor;

namespace MantenimientoFlota.Application.Interfaces;

public interface IProveedorService
{
    Task<Result<IReadOnlyList<ProveedorDto>>> GetAllAsync();

    Task<Result<ProveedorDetalleDto>> GetByIdAsync(int id);

    Task<Result<ProveedorDto>> CreateAsync(CrearProveedorDto dto);

    Task<Result<object?>> UpdateAsync(
        int id,
        ActualizarProveedorDto dto);

    Task<Result<object?>> DeleteAsync(int id);
}
