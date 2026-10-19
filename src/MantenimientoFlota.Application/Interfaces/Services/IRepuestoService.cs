using MantenimientoFlota.Application.Common.Results;
using MantenimientoFlota.Application.DTOs.Repuesto;

namespace MantenimientoFlota.Application.Interfaces.Services;

public interface IRepuestoService
{
    Task<IReadOnlyList<RepuestoDto>> GetAllAsync(bool stockBajo = false);

    Task<RepuestoDto?> GetByIdAsync(int id);

    Task<RepuestoOperationResult> CreateAsync(CrearRepuestoDto dto);

    Task<RepuestoOperationResult> UpdateAsync(
        int id,
        ActualizarRepuestoDto dto);

    Task<RepuestoOperationResult> DeleteAsync(int id);
}
