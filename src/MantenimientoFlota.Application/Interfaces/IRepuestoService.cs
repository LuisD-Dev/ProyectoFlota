using MantenimientoFlota.Domain.DTO.PatternResult;
using MantenimientoFlota.Domain.DTO.Repuesto;

namespace MantenimientoFlota.Application.Interfaces;

public interface IRepuestoService
{
    Task<Result<IReadOnlyList<RepuestoDto>>> GetAllAsync(bool stockBajo = false);

    Task<Result<RepuestoDetalleDto>> GetByIdAsync(int id);

    Task<Result<RepuestoDto>> CreateAsync(CrearRepuestoDto dto);

    Task<Result<object?>> UpdateAsync(
        int id,
        ActualizarRepuestoDto dto);

    Task<Result<object?>> DeleteAsync(int id);
}
