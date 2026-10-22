using MantenimientoFlota.Domain.DTO.PatternResult;
using MantenimientoFlota.Domain.DTO.OrdenMantenimiento;

namespace MantenimientoFlota.Application.Interfaces;

public interface IOrdenMantenimientoService
{
    Task<Result<IReadOnlyList<OrdenMantenimientoDto>>> GetAllAsync(int? activoId = null);

    Task<Result<OrdenMantenimientoDetalleDto>> GetByIdAsync(int id);

    Task<Result<OrdenMantenimientoDto>> CreateAsync(
        CrearOrdenMantenimientoDto dto);
}
