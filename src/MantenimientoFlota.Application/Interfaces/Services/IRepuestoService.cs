using MantenimientoFlota.Application.DTOs.Repuesto;

namespace MantenimientoFlota.Application.Interfaces.Services;

public interface IRepuestoService
{
    Task<IReadOnlyList<RepuestoDto>> GetAllAsync(bool stockBajo = false);

    Task<RepuestoDetalleDto> GetByIdAsync(int id);

    Task<RepuestoDto> CreateAsync(CrearRepuestoDto dto);

    Task UpdateAsync(
        int id,
        ActualizarRepuestoDto dto);

    Task DeleteAsync(int id);
}
