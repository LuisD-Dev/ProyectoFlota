using MantenimientoFlota.Domain.DTO.Tecnico;
using MantenimientoFlota.Domain.Entities;

namespace MantenimientoFlota.Infrastructure.Interfaces;

public interface ITecnicoRepository
{
    Task<IEnumerable<Tecnico>> GetAllAsync(
        bool? disponible = null,
        int? especialidadId = null);

    Task<Tecnico?> GetForUpdateAsync(int id);

    Task<Tecnico?> GetByIdAsync(int id);

    Task<(Tecnico? Entidad, int Cantidad)> GetDetalleByIdAsync(int id);

    Task<Tecnico?> AddAsync(Tecnico tecnico);

    Task UpdateAsync(Tecnico tecnico);

    Task DeleteAsync(int id);

    Task<bool> HasAssociatedSpecialtiesAsync(int tecnicoId);
}
