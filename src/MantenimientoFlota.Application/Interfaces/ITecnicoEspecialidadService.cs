using MantenimientoFlota.Application.Common.Results;
using MantenimientoFlota.Application.DTOs.TecnicoEspecialidad;

namespace MantenimientoFlota.Application.Interfaces;

public interface ITecnicoEspecialidadService
{
    Task<IReadOnlyList<TecnicoEspecialidadDto>> GetAllAsync();

    Task<TecnicoEspecialidadDto?> GetByIdAsync(
        int tecnicoId,
        int especialidadId);

    Task<TecnicoEspecialidadOperationResult> CreateAsync(
        CrearTecnicoEspecialidadDto dto);

    Task<TecnicoEspecialidadOperationResult> UpdateAsync(
        int tecnicoId,
        int especialidadId,
        ActualizarTecnicoEspecialidadDto dto);

    Task<TecnicoEspecialidadOperationResult> DeleteAsync(
        int tecnicoId,
        int especialidadId);
}
