using MantenimientoFlota.Application.DTOs.Tecnico;

namespace MantenimientoFlota.Application.Interfaces.Services;

public interface ITecnicoService
{
    Task<IEnumerable<TecnicoDto>> GetAllAsync(
        bool? disponible = null,
        int? especialidadId = null);

    Task<TecnicoDetalleDto> GetByIdAsync(int id);

    Task<TecnicoDto> CreateAsync(CrearTecnicoDto dto);

    Task UpdateAsync(
        int id,
        ActualizarTecnicoDto dto);

    Task DeleteAsync(int id);
}
