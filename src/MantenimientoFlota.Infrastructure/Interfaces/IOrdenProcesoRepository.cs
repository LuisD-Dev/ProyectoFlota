using MantenimientoFlota.Domain.Entities;
namespace MantenimientoFlota.Infrastructure.Interfaces;

public interface IOrdenProcesoRepository
{
    Task<T> EjecutarAsync<T>(Func<Task<T>> operacion);
    Task<OrdenMantenimiento?> GetForUpdateAsync(int id);
    Task<Especialidad?> GetEspecialidadAsync(int id);
    Task<Tecnico?> GetTecnicoAsync(int id);
    Task<bool> TieneEspecialidadAsync(int tecnicoId, int especialidadId);
    Task<bool> TieneOtraOrdenEnEjecucionAsync(int activoId, int excluirId);
    Task<Repuesto?> GetRepuestoAsync(int id);
    Task<decimal?> GetPrecioAsync(int repuestoId);
    Task<List<RepuestoOrden>> GetRepuestosAsync(int ordenId);
    Task<List<TecnicoOrden>> GetTecnicosAsync(int ordenId);
    Task<List<HistorialEstado>> GetHistorialAsync(int? ordenId, int? activoId, int? usuarioSolicitanteId);
    void Agregar(HistorialEstado historial);
    void Agregar(RepuestoOrden repuesto);
    void Agregar(TecnicoOrden tecnico);
}
