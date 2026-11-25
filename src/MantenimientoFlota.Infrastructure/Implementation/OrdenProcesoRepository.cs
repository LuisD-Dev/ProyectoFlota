using System.Data;
using MantenimientoFlota.Domain.Entities;
using MantenimientoFlota.Infrastructure.Data;
using MantenimientoFlota.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace MantenimientoFlota.Infrastructure.Implementation;

public sealed class OrdenProcesoRepository(AppDbContext context) : IOrdenProcesoRepository
{
    // Todos los pasos del servicio usan el mismo DbContext y una sola transacción.
    // Serializable evita aprobar consumos simultáneos sobre el mismo stock.
    public async Task<T> EjecutarAsync<T>(Func<Task<T>> operacion)
    {
        var strategy = new SqlServerRetryingExecutionStrategy(context, 3, TimeSpan.FromSeconds(1), [1205]);
        return await strategy.ExecuteAsync(async () =>
        {
            context.ChangeTracker.Clear();
            await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var result = await operacion();
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return result;
            // Si cualquier paso falla, Dispose revierte toda la transacción.
        });
    }

    public Task<OrdenMantenimiento?> GetForUpdateAsync(int id) =>
        context.OrdenesMantenimiento.Include(x => x.Activo).SingleOrDefaultAsync(x => x.Id == id);
    public Task<Especialidad?> GetEspecialidadAsync(int id) => context.Especialidades.SingleOrDefaultAsync(x => x.Id == id);
    public Task<Tecnico?> GetTecnicoAsync(int id) => context.Tecnicos.SingleOrDefaultAsync(x => x.Id == id);
    public Task<bool> TieneEspecialidadAsync(int tecnicoId, int especialidadId) =>
        context.TecnicoEspecialidades.AnyAsync(x => x.TecnicoId == tecnicoId && x.EspecialidadId == especialidadId);
    public Task<bool> TieneOtraOrdenEnEjecucionAsync(int activoId, int excluirId) =>
        context.OrdenesMantenimiento.AnyAsync(x => x.ActivoId == activoId && x.Id != excluirId &&
            (x.EstadoActual == 4 || x.EstadoActual == 5));
    public Task<Repuesto?> GetRepuestoAsync(int id) => context.Repuestos.SingleOrDefaultAsync(x => x.Id == id);
    public Task<decimal?> GetPrecioAsync(int repuestoId) => context.RepuestoProveedores
        .Where(x => x.RepuestoId == repuestoId).OrderBy(x => x.PrecioOfrecido)
        .ThenBy(x => x.TiempoEntregaDias).ThenBy(x => x.ProveedorId)
        .Select(x => (decimal?)x.PrecioOfrecido).FirstOrDefaultAsync();
    public Task<List<RepuestoOrden>> GetRepuestosAsync(int ordenId) =>
        context.Set<RepuestoOrden>().Include(x => x.Repuesto).Where(x => x.OrdenMantenimientoId == ordenId).OrderBy(x => x.Id).ToListAsync();
    public Task<List<TecnicoOrden>> GetTecnicosAsync(int ordenId) =>
        context.Set<TecnicoOrden>().Include(x => x.Tecnico).Where(x => x.OrdenMantenimientoId == ordenId).OrderBy(x => x.TecnicoId).ToListAsync();
    public Task<List<HistorialEstado>> GetHistorialAsync(int? ordenId, int? activoId, int? usuarioSolicitanteId) =>
        context.Set<HistorialEstado>().AsNoTracking().Where(x =>
            (ordenId != null && x.OrdenMantenimientoId == ordenId) ||
            (activoId != null && x.ActivoId == activoId && (usuarioSolicitanteId == null || x.UsuarioId == usuarioSolicitanteId)))
            .OrderBy(x => x.FechaUtc).ThenBy(x => x.Id).ToListAsync();
    public void Agregar(HistorialEstado historial) => context.Add(historial);
    public void Agregar(RepuestoOrden repuesto) => context.Add(repuesto);
    public void Agregar(TecnicoOrden tecnico) => context.Add(tecnico);
}
