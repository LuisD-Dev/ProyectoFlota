using MantenimientoFlota.Application.Common.Results;
using MantenimientoFlota.Application.DTOs.Activo;

namespace MantenimientoFlota.Application.Interfaces;

public interface IActivoService
{
    Task<IReadOnlyList<ActivoDto>> GetAllAsync();

    Task<ActivoDto?> GetByIdAsync(int id);

    Task<ActivoOperationResult> CreateAsync(CrearActivoDto dto);

    Task<ActivoOperationResult> UpdateAsync(
        int id,
        ActualizarActivoDto dto);

    Task<ActivoOperationResult> DeleteAsync(int id);
}
