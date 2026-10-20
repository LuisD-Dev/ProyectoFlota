using MantenimientoFlota.Domain.Entities;
using MantenimientoFlota.Application.DTOs.Tecnico;

namespace MantenimientoFlota.Application.Interfaces.Repositories;

public interface ITecnicoRepository
{
    Task<IEnumerable<Tecnico>> GetAllAsync(
        bool? disponible = null,
        int? especialidadId = null);

    Task<Tecnico?> GetByIdAsync(int id);

    Task<TecnicoDetalleDto?> GetDetalleByIdAsync(int id);

    Task<Tecnico?> AddAsync(Tecnico tecnico);

    void Update(Tecnico tecnico);

    void Delete(int id);

    Task<bool> HasAssociatedSpecialtiesAsync(int tecnicoId);
}
