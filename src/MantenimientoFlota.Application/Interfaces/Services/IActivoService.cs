using MantenimientoFlota.Application.DTOs.Activo;
using MantenimientoFlota.Domain.Enums;

namespace MantenimientoFlota.Application.Interfaces.Services;

public interface IActivoService
{
    Task<IReadOnlyList<ActivoDto>> GetAllAsync(
        int? categoriaActivoId = null,
        EstadoActivo? estado = null);

    Task<ActivoDetalleDto> GetByIdAsync(int id);

    Task<ActivoDto> CreateAsync(CrearActivoDto dto);

    Task UpdateAsync(
        int id,
        ActualizarActivoDto dto);

    Task CambiarEstadoAdministrativoAsync(int id, CambiarEstadoActivoDto dto);

    Task DeleteAsync(int id);

    Task<ActivoImagenDto> ActualizarImagenAsync(int id, Stream contenido, string extension, CancellationToken cancellationToken);
}
