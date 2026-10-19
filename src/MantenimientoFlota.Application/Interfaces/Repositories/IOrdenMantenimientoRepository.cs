using MantenimientoFlota.Application.DTOs.OrdenMantenimiento;
using MantenimientoFlota.Domain.Entities;

namespace MantenimientoFlota.Application.Interfaces.Repositories;

public interface IOrdenMantenimientoRepository
{
    Task<IReadOnlyList<OrdenMantenimientoDto>> GetAllAsync(int? activoId = null);

    Task<OrdenMantenimientoDto?> GetByIdAsync(int id);

    Task<OrdenMantenimiento> AddAsync(
        OrdenMantenimiento ordenMantenimiento);

    Task<bool> ActivoExistsAsync(int activoId);
}
