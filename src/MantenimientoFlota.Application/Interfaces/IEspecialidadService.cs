using MantenimientoFlota.Application.Common.Results;
using MantenimientoFlota.Application.DTOs.Especialidad;

namespace MantenimientoFlota.Application.Interfaces;

public interface IEspecialidadService
{
    Task<IReadOnlyList<EspecialidadDto>> GetAllAsync();

    Task<EspecialidadDto?> GetByIdAsync(int id);

    Task<EspecialidadOperationResult> CreateAsync(CrearEspecialidadDto dto);

    Task<EspecialidadOperationResult> UpdateAsync(
        int id,
        ActualizarEspecialidadDto dto);

    Task<EspecialidadOperationResult> DeleteAsync(int id);
}
