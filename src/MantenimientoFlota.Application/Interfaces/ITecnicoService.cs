using MantenimientoFlota.Application.Common.Results;
using MantenimientoFlota.Application.DTOs.Tecnico;

namespace MantenimientoFlota.Application.Interfaces;

public interface ITecnicoService
{
    Task<IEnumerable<TecnicoDto>> GetAllAsync();

    Task<TecnicoDto?> GetByIdAsync(int id);

    Task<TecnicoOperationResult> CreateAsync(CrearTecnicoDto dto);

    Task<TecnicoOperationResult> UpdateAsync(
        int id,
        ActualizarTecnicoDto dto);

    Task<TecnicoOperationResult> DeleteAsync(int id);
}
