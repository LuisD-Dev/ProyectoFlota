using MantenimientoFlota.Application.DTOs.CategoriaActivo;

namespace MantenimientoFlota.Application.Interfaces.Services;

public interface ICategoriaActivoService
{
    Task<IReadOnlyList<CategoriaActivoDto>> GetAllAsync();

    Task<CategoriaActivoDetalleDto> GetByIdAsync(int id);

    Task<CategoriaActivoDto> CreateAsync(
        CrearCategoriaActivoDto dto);

    Task UpdateAsync(
        int id,
        ActualizarCategoriaActivoDto dto);

    Task DeleteAsync(int id);
}
