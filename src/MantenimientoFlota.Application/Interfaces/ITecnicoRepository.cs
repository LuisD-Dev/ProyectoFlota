using MantenimientoFlota.Domain.Entities;

namespace MantenimientoFlota.Application.Interfaces;

public interface ITecnicoRepository
{
    Task<IEnumerable<Tecnico>> GetAllAsync();

    Task<Tecnico?> GetByIdAsync(int id);

    Task<Tecnico?> AddAsync(Tecnico tecnico);

    void Update(Tecnico tecnico);

    void Delete(int id);

    Task<bool> HasAssociatedSpecialtiesAsync(int tecnicoId);
}
