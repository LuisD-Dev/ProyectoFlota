using MantenimientoFlota.Infrastructure.Interfaces;
using MantenimientoFlota.Domain.Entities;
using MantenimientoFlota.Domain.Constants;
using MantenimientoFlota.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using MantenimientoFlota.Infrastructure.Exceptions;

namespace MantenimientoFlota.Infrastructure.Implementation;

public class ActivoRepository : IActivoRepository
{
    private readonly AppDbContext _context;

    public ActivoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Activo>> GetAllAsync(
        int? categoriaActivoId = null,
        int? estado = null)
    {
        IQueryable<Activo> query = _context.Activos
            .AsNoTracking()
            .Include(activo => activo.CategoriaActivo);

        if (categoriaActivoId.HasValue)
        {
            query = query.Where(activo => activo.CategoriaActivoId == categoriaActivoId.Value);
        }

        if (estado.HasValue)
        {
            query = query.Where(activo => activo.Estado == estado.Value);
        }

        return await query.ToListAsync();
    }

    public Task<Activo?> GetForUpdateAsync(int id) =>
        _context.Activos.FirstOrDefaultAsync(entidad => entidad.Id == id);

    public async Task<Activo?> GetByIdAsync(int id)
    {
        return await _context.Activos
            .AsNoTracking()
            .Include(activo => activo.CategoriaActivo)
            .FirstOrDefaultAsync(activo => activo.Id == id);
    }

    public async Task<Activo?> GetDetalleByIdAsync(int id)
    {
        return await _context.Activos
            .AsNoTracking()
            .AsSingleQuery()
            .Include(activo => activo.CategoriaActivo)
            .Include(activo => activo.OrdenesMantenimiento)
            .FirstOrDefaultAsync(activo => activo.Id == id);
    }

    public async Task<Activo?> AddAsync(Activo activo)
    {
        await _context.Activos.AddAsync(activo);
        await _context.SaveChangesAsync();

        return activo;
    }

    public async Task UpdateAsync(Activo activo)
    {
        if (_context.Entry(activo).State == EntityState.Detached)
            _context.Activos.Update(activo);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateEstadoAsync(Activo activo)
    {
        _context.Entry(activo).Property(entidad => entidad.Estado).IsModified = true;
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarImagenUrlAsync(int id, string imagenUrl, CancellationToken cancellationToken)
    {
        var activo = new Activo { Id = id, ImagenUrl = imagenUrl };
        _context.Entry(activo).Property(entidad => entidad.ImagenUrl).IsModified = true;
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sql
            && sql.Number is 515 or 547 or 2601 or 2627 or 2628 or 8152)
        {
            throw new ImagenUpdateRejectedException(ex);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ImagenUpdateRejectedException(ex);
        }
    }

    public async Task<bool> ExistsByCodigoAsync(
        string codigo,
        int? idExcluir = null)
    {
        var query = _context.Activos
            .AsNoTracking()
            .Where(activo => activo.Codigo == codigo);

        if (idExcluir.HasValue)
        {
            query = query.Where(activo => activo.Id != idExcluir.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<bool> CategoriaExisteAsync(int categoriaActivoId)
    {
        return await _context.CategoriasActivo
            .AsNoTracking()
            .AnyAsync(categoria => categoria.Id == categoriaActivoId);
    }

    public async Task<bool> HasOpenMaintenanceOrdersAsync(int activoId)
    {
        return await _context.OrdenesMantenimiento
            .AsNoTracking()
            .AnyAsync(orden => orden.ActivoId == activoId
                && (orden.EstadoActual == EstadoOrdenMantenimiento.Solicitada
                    || orden.EstadoActual == EstadoOrdenMantenimiento.Diagnosticada
                    || orden.EstadoActual == EstadoOrdenMantenimiento.Aprobada
                    || orden.EstadoActual == EstadoOrdenMantenimiento.EnEjecucion
                    || orden.EstadoActual == EstadoOrdenMantenimiento.EnEsperaDeRepuesto));
    }

    public async Task<bool> HasAssociatedMaintenanceOrdersAsync(int activoId)
    {
        return await _context.OrdenesMantenimiento
            .AsNoTracking()
            .AnyAsync(orden => orden.ActivoId == activoId);
    }
}
