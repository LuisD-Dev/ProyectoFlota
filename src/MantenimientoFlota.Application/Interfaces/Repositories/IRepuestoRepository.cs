using MantenimientoFlota.Domain.Entities;
using MantenimientoFlota.Application.DTOs.Repuesto;

namespace MantenimientoFlota.Application.Interfaces.Repositories;

public interface IRepuestoRepository
{
    Task<IEnumerable<Repuesto>> GetAllAsync(bool stockBajo = false);

    Task<Repuesto?> GetByIdAsync(int id);

    Task<RepuestoDetalleDto?> GetDetalleByIdAsync(int id);

    Task<Repuesto?> AddAsync(Repuesto repuesto);

    void Update(Repuesto repuesto);

    void Delete(int id);

    Task<bool> ExistsByCodigoAsync(string codigo, int? idExcluir = null);

    Task<bool> HasAssociatedSuppliersAsync(int repuestoId);
}
