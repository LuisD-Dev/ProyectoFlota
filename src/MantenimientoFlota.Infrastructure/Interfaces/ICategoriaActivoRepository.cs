using MantenimientoFlota.Domain.DTO.CategoriaActivo;
using MantenimientoFlota.Domain.Entities;

namespace MantenimientoFlota.Infrastructure.Interfaces;

public interface ICategoriaActivoRepository
{
    Task<IEnumerable<CategoriaActivo>> GetAllAsync();

    Task<CategoriaActivo?> GetForUpdateAsync(int id);

    Task<CategoriaActivo?> GetByIdAsync(int id);

    Task<CategoriaActivoDetalleDto?> GetDetalleByIdAsync(int id);

    Task<CategoriaActivo?> AddAsync(CategoriaActivo categoria);

    Task UpdateAsync(CategoriaActivo categoria);

    Task DeleteAsync(int id);

    Task<bool> ExistsByNameAsync(string nombre, int? idExcluir = null);

    Task<bool> HasAssociatedAssetsAsync(int categoriaActivoId);
}
