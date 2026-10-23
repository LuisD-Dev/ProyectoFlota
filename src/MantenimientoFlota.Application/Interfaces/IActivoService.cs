using MantenimientoFlota.Domain.DTO.PatternResult;
using MantenimientoFlota.Domain.DTO.Activo;
using MantenimientoFlota.Domain.Constants;

namespace MantenimientoFlota.Application.Interfaces;

public interface IActivoService
{
    Task<Result<IReadOnlyList<ActivoDto>>> SearchAsync(BuscarActivosDto consulta);

    Task<Result<IReadOnlyList<ActivoDto>>> GetAllAsync(
        int? categoriaActivoId = null,
        int? estado = null);

    Task<Result<ActivoDetalleDto>> GetByIdAsync(int id);

    Task<Result<ActivoDto>> CreateAsync(CrearActivoDto dto);

    Task<Result<object?>> UpdateAsync(
        int id,
        ActualizarActivoDto dto);

    Task<Result<object?>> CambiarEstadoAdministrativoAsync(int id, CambiarEstadoActivoDto dto);

    Task<Result<object?>> DeleteAsync(int id);

    Task<Result<ActivoImagenDto>> ActualizarImagenAsync(int id, Stream contenido, string extension, CancellationToken cancellationToken);
}
