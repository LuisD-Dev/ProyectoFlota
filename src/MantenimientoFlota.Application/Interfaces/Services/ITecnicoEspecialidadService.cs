using MantenimientoFlota.Application.DTOs.TecnicoEspecialidad;

namespace MantenimientoFlota.Application.Interfaces.Services;

public interface ITecnicoEspecialidadService
{
    Task<IReadOnlyList<TecnicoEspecialidadDto>> GetAllAsync();

    Task<TecnicoEspecialidadDto> GetByIdAsync(
        int tecnicoId,
        int especialidadId);

    Task<TecnicoEspecialidadDto> CreateAsync(
        CrearTecnicoEspecialidadDto dto);

    Task UpdateAsync(
        int tecnicoId,
        int especialidadId,
        ActualizarTecnicoEspecialidadDto dto);

    Task DeleteAsync(
        int tecnicoId,
        int especialidadId);
}
