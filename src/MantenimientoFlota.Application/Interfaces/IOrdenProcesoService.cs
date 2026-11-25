using MantenimientoFlota.Domain.DTO.OrdenMantenimiento;
using MantenimientoFlota.Domain.DTO.PatternResult;
namespace MantenimientoFlota.Application.Interfaces;

public interface IOrdenProcesoService
{
    Task<Result<object?>> CambiarEstadoAsync(int id, CambiarEstadoOrdenDto dto);
    Task<Result<object?>> RegistrarRepuestosAsync(int id, RegistrarRepuestosDto dto);
    Task<Result<object?>> AsignarTecnicoAsync(int id, AsignarTecnicoDto dto);
    Task<Result<IReadOnlyList<RepuestoOrdenDto>>> GetRepuestosAsync(int id);
    Task<Result<IReadOnlyList<TecnicoOrdenDto>>> GetTecnicosAsync(int id);
    Task<Result<IReadOnlyList<HistorialEstadoDto>>> GetHistorialAsync(int id);
    Task<Result<IReadOnlyList<HistorialEstadoDto>>> GetHistorialActivoAsync(int id);
}
