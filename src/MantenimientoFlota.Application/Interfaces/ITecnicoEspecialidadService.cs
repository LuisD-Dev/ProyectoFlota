using MantenimientoFlota.Domain.DTO.PatternResult;
using MantenimientoFlota.Domain.DTO.TecnicoEspecialidad;

namespace MantenimientoFlota.Application.Interfaces;

public interface ITecnicoEspecialidadService
{
    Task<Result<IReadOnlyList<TecnicoEspecialidadDto>>> GetAllAsync(int tecnicoId);

    Task<Result<TecnicoEspecialidadDto>> GetByIdAsync(
        int tecnicoId,
        int especialidadId);

    Task<Result<TecnicoEspecialidadDto>> CreateAsync(
        int tecnicoId,
        CrearTecnicoEspecialidadDto dto);

    Task<Result<object?>> UpdateAsync(
        int tecnicoId,
        int especialidadId,
        ActualizarTecnicoEspecialidadDto dto);

    Task<Result<object?>> DeleteAsync(
        int tecnicoId,
        int especialidadId);
}
