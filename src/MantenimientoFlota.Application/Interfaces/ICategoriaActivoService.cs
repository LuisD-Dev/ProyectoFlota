using MantenimientoFlota.Domain.DTO.PatternResult;
using MantenimientoFlota.Domain.DTO.CategoriaActivo;

namespace MantenimientoFlota.Application.Interfaces;

public interface ICategoriaActivoService
{
    Task<Result<IReadOnlyList<CategoriaActivoDto>>> GetAllAsync();

    Task<Result<CategoriaActivoDetalleDto>> GetByIdAsync(int id);

    Task<Result<CategoriaActivoDto>> CreateAsync(
        CrearCategoriaActivoDto dto);

    Task<Result<object?>> UpdateAsync(
        int id,
        ActualizarCategoriaActivoDto dto);

    Task<Result<object?>> DeleteAsync(int id);
}
