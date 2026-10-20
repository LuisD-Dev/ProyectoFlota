using MantenimientoFlota.Domain.Entities;
using MantenimientoFlota.Application.DTOs.CategoriaActivo;

namespace MantenimientoFlota.Application.Interfaces.Repositories;

public interface ICategoriaActivoRepository
{
    Task<IEnumerable<CategoriaActivo>> GetAllAsync();

    Task<CategoriaActivo?> GetByIdAsync(int id);

    Task<CategoriaActivoDetalleDto?> GetDetalleByIdAsync(int id);

    Task<CategoriaActivo?> AddAsync(CategoriaActivo categoria);

    void Update(CategoriaActivo categoria);

    void Delete(int id);

    Task<bool> ExistsByNameAsync(string nombre, int? idExcluir = null);

    Task<bool> HasAssociatedAssetsAsync(int categoriaActivoId);
}
