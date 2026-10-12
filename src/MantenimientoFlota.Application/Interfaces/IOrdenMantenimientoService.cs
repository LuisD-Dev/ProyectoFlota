using MantenimientoFlota.Application.Common.Results;
using MantenimientoFlota.Application.DTOs.OrdenMantenimiento;

namespace MantenimientoFlota.Application.Interfaces;

public interface IOrdenMantenimientoService
{
    Task<IReadOnlyList<OrdenMantenimientoDto>> GetAllAsync();

    Task<OrdenMantenimientoDto?> GetByIdAsync(int id);

    Task<OrdenMantenimientoOperationResult> CreateAsync(
        CrearOrdenMantenimientoDto dto);
}
