using MantenimientoFlota.Domain.DTO.Repuesto;
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

    public async Task<(IEnumerable<Repuesto> Items, int TotalRecords)> SearchAsync(BuscarRepuestosDto consulta)
    {
        IQueryable<Repuesto> query = _context.Repuestos.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(consulta.Texto))
        {
            var texto = consulta.Texto.Trim();
            query = query.Where(x => x.Codigo.Contains(texto) || x.Nombre.Contains(texto));
        }
        if (consulta.ProveedorId.HasValue) query = query.Where(x => x.RepuestoProveedores.Any(r => r.ProveedorId == consulta.ProveedorId.Value));
        if (consulta.StockBajo.HasValue) query = query.Where(x => (x.StockActual < x.StockMinimo) == consulta.StockBajo.Value);
        if (consulta.StockDesde.HasValue) query = query.Where(x => x.StockActual >= consulta.StockDesde.Value);
        if (consulta.StockHasta.HasValue) query = query.Where(x => x.StockActual <= consulta.StockHasta.Value);
        var totalRecords = await query.CountAsync();
        var descending = string.Equals(consulta.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);
        IOrderedQueryable<Repuesto> ordered = consulta.SortBy.ToLowerInvariant() switch
        {
            "codigo" => descending ? query.OrderByDescending(x => x.Codigo) : query.OrderBy(x => x.Codigo),
            "stock" => descending ? query.OrderByDescending(x => x.StockActual) : query.OrderBy(x => x.StockActual),
            "minimo" => descending ? query.OrderByDescending(x => x.StockMinimo) : query.OrderBy(x => x.StockMinimo),
            _ => descending ? query.OrderByDescending(x => x.Nombre) : query.OrderBy(x => x.Nombre)
        };
        var offset = ((long)consulta.PageNumber - 1) * consulta.PageSize;
        if (offset > int.MaxValue) return (Array.Empty<Repuesto>(), totalRecords);
        var items = await ordered.ThenBy(x => x.Id).Skip((int)offset).Take(consulta.PageSize).ToListAsync();
        return (items, totalRecords);
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

    public async Task<(Repuesto? Entidad, int Cantidad)> GetDetalleByIdAsync(int id)
    {
        // Count se ejecuta en SQL; Application transforma la entidad al DTO.
        var resultado = await _context.Repuestos
            .AsNoTracking()
            .AsSingleQuery()
            .Include(entidad => entidad.RepuestoProveedores)
            .ThenInclude(relacion => relacion.Proveedor)
            .Where(entidad => entidad.Id == id)
            .Select(entidad => new { Entidad = entidad, Cantidad = entidad.RepuestoProveedores.Count() })
            .FirstOrDefaultAsync();
        return (resultado?.Entidad, resultado?.Cantidad ?? 0);
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
