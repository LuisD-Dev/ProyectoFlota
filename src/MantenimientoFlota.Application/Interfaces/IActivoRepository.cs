using MantenimientoFlota.Domain.Entities;

namespace MantenimientoFlota.Application.Interfaces;

public interface IActivoRepository
{
    Task<IEnumerable<Activo>> GetAllAsync();

    Task<Activo?> GetByIdAsync(int id);

    Task<Activo?> AddAsync(Activo activo);

    void Update(Activo activo);

    void Delete(int id);

    Task<bool> ExistsByCodigoAsync(string codigo, int? idExcluir = null);

    Task<bool> CategoriaExisteAsync(int categoriaActivoId);
}
