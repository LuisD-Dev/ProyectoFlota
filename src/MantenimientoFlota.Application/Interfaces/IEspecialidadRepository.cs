using MantenimientoFlota.Domain.Entities;

namespace MantenimientoFlota.Application.Interfaces;

public interface IEspecialidadRepository
{
    Task<IEnumerable<Especialidad>> GetAllAsync();

    Task<Especialidad?> GetByIdAsync(int id);

    Task<Especialidad> AddAsync(Especialidad especialidad);

    Task UpdateAsync(Especialidad especialidad);

    Task DeleteAsync(Especialidad especialidad);

    Task<bool> ExistsByNameAsync(string nombre, int? idExcluir = null);

    Task<bool> HasAssociatedTechniciansAsync(int especialidadId);
}
