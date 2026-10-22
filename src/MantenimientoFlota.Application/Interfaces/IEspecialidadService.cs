using MantenimientoFlota.Domain.DTO.PatternResult;
using MantenimientoFlota.Domain.DTO.Especialidad;

namespace MantenimientoFlota.Application.Interfaces;

public interface IEspecialidadService
{
    Task<Result<IReadOnlyList<EspecialidadDto>>> GetAllAsync();

    Task<Result<EspecialidadDetalleDto>> GetByIdAsync(int id);

    Task<Result<EspecialidadDto>> CreateAsync(CrearEspecialidadDto dto);

    Task<Result<object?>> UpdateAsync(
        int id,
        ActualizarEspecialidadDto dto);

    Task<Result<object?>> DeleteAsync(int id);
}
