using MantenimientoFlota.Application.Common.Results;
using MantenimientoFlota.Application.DTOs.Repuesto;

namespace MantenimientoFlota.Application.Interfaces;

public interface IRepuestoService
{
    Task<IReadOnlyList<RepuestoDto>> GetAllAsync();

    Task<RepuestoDto?> GetByIdAsync(int id);

    Task<RepuestoOperationResult> CreateAsync(CrearRepuestoDto dto);

    Task<RepuestoOperationResult> UpdateAsync(
        int id,
        ActualizarRepuestoDto dto);

    Task<RepuestoOperationResult> DeleteAsync(int id);
}
