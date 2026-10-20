using MantenimientoFlota.Application.Common.Results;
using MantenimientoFlota.Application.DTOs.Activo;
using MantenimientoFlota.Domain.Enums;

namespace MantenimientoFlota.Application.Interfaces.Services;

public interface IActivoService
{
    Task<IReadOnlyList<ActivoDto>> GetAllAsync(
        int? categoriaActivoId = null,
        EstadoActivo? estado = null);

    Task<ActivoDetalleDto?> GetByIdAsync(int id);

    Task<ActivoOperationResult> CreateAsync(CrearActivoDto dto);

    Task<ActivoOperationResult> UpdateAsync(
        int id,
        ActualizarActivoDto dto);

    Task<ActivoOperationResult> DeleteAsync(int id);
}
