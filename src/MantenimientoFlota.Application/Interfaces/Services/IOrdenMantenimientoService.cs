using MantenimientoFlota.Application.DTOs.OrdenMantenimiento;

namespace MantenimientoFlota.Application.Interfaces.Services;

public interface IOrdenMantenimientoService
{
    Task<IReadOnlyList<OrdenMantenimientoDto>> GetAllAsync(int? activoId = null);

    Task<OrdenMantenimientoDetalleDto> GetByIdAsync(int id);

    Task<OrdenMantenimientoDto> CreateAsync(
        CrearOrdenMantenimientoDto dto);
}
