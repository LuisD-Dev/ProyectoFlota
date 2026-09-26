using MantenimientoFlota.Application.Interfaces;
using MantenimientoFlota.Domain.Entities;

namespace MantenimientoFlota.Application.Implementation;

public class CategoriaActivoService : ICategoriaActivoService
{
    private readonly ICategoriaActivoRepository _repository;

    public CategoriaActivoService(ICategoriaActivoRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<CategoriaActivo>> GetAllAsync()
    {
        return _repository.GetAllAsync();
    }

    public Task<CategoriaActivo?> GetByIdAsync(int id)
    {
        return _repository.GetByIdAsync(id);
    }

    public Task<CategoriaActivo?> AddAsync(CategoriaActivo categoria)
    {
        return _repository.AddAsync(categoria);
    }

    public void Update(CategoriaActivo categoria)
    {
        _repository.Update(categoria);
    }

    public void Delete(int id)
    {
        _repository.Delete(id);
    }

    public Task<bool> ExistsByNameAsync(string nombre, int? idExcluir = null)
    {
        return _repository.ExistsByNameAsync(nombre, idExcluir);
    }
}
