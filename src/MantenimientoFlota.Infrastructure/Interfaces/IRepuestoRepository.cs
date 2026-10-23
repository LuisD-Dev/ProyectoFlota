using MantenimientoFlota.Domain.DTO.Repuesto;
using MantenimientoFlota.Domain.Entities;

namespace MantenimientoFlota.Infrastructure.Interfaces;

public interface IRepuestoRepository
{
    Task<(IEnumerable<Repuesto> Items, int TotalRecords)> SearchAsync(BuscarRepuestosDto consulta);

    Task<IEnumerable<Repuesto>> GetAllAsync(bool stockBajo = false);

    Task<Repuesto?> GetForUpdateAsync(int id);

    Task<Repuesto?> GetByIdAsync(int id);

    Task<(Repuesto? Entidad, int Cantidad)> GetDetalleByIdAsync(int id);

    Task<Repuesto?> AddAsync(Repuesto repuesto);

    Task UpdateAsync(Repuesto repuesto);

    Task DeleteAsync(int id);

    Task<bool> ExistsByCodigoAsync(string codigo, int? idExcluir = null);

    Task<bool> HasAssociatedSuppliersAsync(int repuestoId);
}
