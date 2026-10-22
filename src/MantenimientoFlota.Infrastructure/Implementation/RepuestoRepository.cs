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

    public async Task<RepuestoDetalleDto?> GetDetalleByIdAsync(int id)
    {
        // El conteo se traduce a COUNT en SQL; las relaciones se proyectan a resúmenes.
        return await _context.Repuestos
            .AsNoTracking()
            .AsSingleQuery()
            .Where(entidad => entidad.Id == id)
            .Select(entidad => new RepuestoDetalleDto
            {
                Id = entidad.Id, Codigo = entidad.Codigo, Nombre = entidad.Nombre,
                UnidadMedida = entidad.UnidadMedida, StockActual = entidad.StockActual, StockMinimo = entidad.StockMinimo,
                StockBajo = entidad.StockActual < entidad.StockMinimo,
                FaltanteStockMinimo = entidad.StockActual < entidad.StockMinimo ? entidad.StockMinimo - entidad.StockActual : 0m,
                CantidadProveedores = entidad.RepuestoProveedores.Count(),
                Proveedores = entidad.RepuestoProveedores.OrderBy(relacion => relacion.Proveedor.Nombre)
                    .ThenBy(relacion => relacion.ProveedorId).Select(relacion => new ProveedorRepuestoResumenDto {
                        ProveedorId = relacion.ProveedorId, Nombre = relacion.Proveedor.Nombre,
                        PrecioOfrecido = relacion.PrecioOfrecido, TiempoEntregaDias = relacion.TiempoEntregaDias }).ToList()
            })
            .FirstOrDefaultAsync();
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
