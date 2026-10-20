using MantenimientoFlota.Application.Common.Results;
using MantenimientoFlota.Application.DTOs.CategoriaActivo;

namespace MantenimientoFlota.Application.Interfaces.Services;

public interface ICategoriaActivoService
{
    Task<IReadOnlyList<CategoriaActivoDto>> GetAllAsync();

    Task<CategoriaActivoDetalleDto?> GetByIdAsync(int id);

    Task<CategoriaActivoOperationResult> CreateAsync(
        CrearCategoriaActivoDto dto);

    Task<CategoriaActivoOperationResult> UpdateAsync(
        int id,
        ActualizarCategoriaActivoDto dto);

    Task<CategoriaActivoOperationResult> DeleteAsync(int id);
}
