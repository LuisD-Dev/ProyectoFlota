using MantenimientoFlota.Infrastructure.Interfaces;
using MantenimientoFlota.Domain.Entities;
using MantenimientoFlota.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MantenimientoFlota.Infrastructure.Implementation;

public class RepuestoRepository : IRepuestoRepository
{
    private readonly AppDbContext _context;

    public RepuestoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Repuesto>> GetAllAsync(bool stockBajo = false)
    {
        IQueryable<Repuesto> query = _context.Repuestos.AsNoTracking();

        if (stockBajo)
        {
            query = query.Where(repuesto => repuesto.StockActual < repuesto.StockMinimo);
        }

        return await query.ToListAsync();
    }

    public Task<Repuesto?> GetForUpdateAsync(int id) =>
        _context.Repuestos.FirstOrDefaultAsync(entidad => entidad.Id == id);

    public async Task<Repuesto?> GetByIdAsync(int id)
    {
        return await _context.Repuestos
            .AsNoTracking()
            .FirstOrDefaultAsync(repuesto => repuesto.Id == id);
    }

    public async Task<Repuesto?> GetDetalleByIdAsync(int id)
    {
        return await _context.Repuestos
            .AsNoTracking()
            .AsSingleQuery()
            .Include(entidad => entidad.RepuestoProveedores)
            .ThenInclude(relacion => relacion.Proveedor)
            .FirstOrDefaultAsync(entidad => entidad.Id == id);
    }

    public async Task<Repuesto?> AddAsync(Repuesto repuesto)
    {
        await _context.Repuestos.AddAsync(repuesto);
        await _context.SaveChangesAsync();

        return repuesto;
    }

    public async Task UpdateAsync(Repuesto repuesto)
    {
        if (_context.Entry(repuesto).State == EntityState.Detached)
            _context.Repuestos.Update(repuesto);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var repuesto = await _context.Repuestos.FindAsync(id);

        if (repuesto is null)
        {
            return;
        }

        _context.Repuestos.Remove(repuesto);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsByCodigoAsync(string codigo, int? idExcluir = null)
    {
        var query = _context.Repuestos
            .AsNoTracking()
            .Where(repuesto => repuesto.Codigo == codigo);

        if (idExcluir.HasValue)
        {
            query = query.Where(repuesto => repuesto.Id != idExcluir.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<bool> HasAssociatedSuppliersAsync(int repuestoId)
    {
        return await _context.RepuestoProveedores
            .AsNoTracking()
            .AnyAsync(repuestoProveedor => repuestoProveedor.RepuestoId == repuestoId);
    }
}
