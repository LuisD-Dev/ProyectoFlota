using MantenimientoFlota.Domain.DTO.PatternResult;
using MantenimientoFlota.Domain.DTO.Tecnico;

namespace MantenimientoFlota.Application.Interfaces;

public interface ITecnicoService
{
    Task<Result<IEnumerable<TecnicoDto>>> GetAllAsync(
        bool? disponible = null,
        int? especialidadId = null);

    Task<Result<TecnicoDetalleDto>> GetByIdAsync(int id);

    Task<Result<TecnicoDto>> CreateAsync(CrearTecnicoDto dto);

    Task<Result<object?>> UpdateAsync(
        int id,
        ActualizarTecnicoDto dto);

    Task<Result<object?>> DeleteAsync(int id);
}
