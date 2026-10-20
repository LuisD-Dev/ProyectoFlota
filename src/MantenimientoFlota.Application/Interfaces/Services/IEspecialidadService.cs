using MantenimientoFlota.Application.DTOs.Especialidad;

namespace MantenimientoFlota.Application.Interfaces.Services;

public interface IEspecialidadService
{
    Task<IReadOnlyList<EspecialidadDto>> GetAllAsync();

    Task<EspecialidadDetalleDto> GetByIdAsync(int id);

    Task<EspecialidadDto> CreateAsync(CrearEspecialidadDto dto);

    Task UpdateAsync(
        int id,
        ActualizarEspecialidadDto dto);

    Task DeleteAsync(int id);
}
