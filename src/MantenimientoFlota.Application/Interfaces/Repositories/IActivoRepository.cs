using MantenimientoFlota.Domain.Entities;
using MantenimientoFlota.Domain.Enums;

namespace MantenimientoFlota.Application.Interfaces.Repositories;

public interface IActivoRepository
{
    Task<IEnumerable<Activo>> GetAllAsync(
        int? categoriaActivoId = null,
        EstadoActivo? estado = null);

    Task<Activo?> GetByIdAsync(int id);

    Task<Activo?> AddAsync(Activo activo);

    void Update(Activo activo);

    void Delete(int id);

    Task<bool> ExistsByCodigoAsync(string codigo, int? idExcluir = null);

    Task<bool> CategoriaExisteAsync(int categoriaActivoId);

    Task<bool> HasAssociatedMaintenanceOrdersAsync(int activoId);
}
