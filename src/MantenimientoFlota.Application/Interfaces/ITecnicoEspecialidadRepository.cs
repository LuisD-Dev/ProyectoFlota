using MantenimientoFlota.Domain.Entities;

namespace MantenimientoFlota.Application.Interfaces;

public interface ITecnicoEspecialidadRepository
{
    Task<IReadOnlyList<TecnicoEspecialidad>> GetAllAsync();

    Task<TecnicoEspecialidad?> GetByIdAsync(
        int tecnicoId,
        int especialidadId);

    Task<TecnicoEspecialidad> AddAsync(
        TecnicoEspecialidad tecnicoEspecialidad);

    Task UpdateAsync(TecnicoEspecialidad tecnicoEspecialidad);

    Task DeleteAsync(int tecnicoId, int especialidadId);

    Task<bool> ExistsAsync(int tecnicoId, int especialidadId);

    Task<bool> TechnicianExistsAsync(int tecnicoId);

    Task<bool> SpecialtyExistsAsync(int especialidadId);
}
