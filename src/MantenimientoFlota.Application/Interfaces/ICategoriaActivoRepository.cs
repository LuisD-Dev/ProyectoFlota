using MantenimientoFlota.Domain.Entities;

namespace MantenimientoFlota.Application.Interfaces;

public interface ICategoriaActivoRepository
{
    Task<IEnumerable<CategoriaActivo>> GetAllAsync();

    Task<CategoriaActivo?> GetByIdAsync(int id);

    Task<CategoriaActivo?> AddAsync(CategoriaActivo categoria);

    void Update(CategoriaActivo categoria);

    void Delete(int id);

    Task<bool> ExistsByNameAsync(string nombre, int? idExcluir = null);

    Task<bool> HasAssociatedAssetsAsync(int categoriaActivoId);
}
