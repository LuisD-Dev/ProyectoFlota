using MantenimientoFlota.Application.Interfaces.Repositories;
using MantenimientoFlota.Application.DTOs.Repuesto;
using MantenimientoFlota.Domain.Entities;
using MantenimientoFlota.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MantenimientoFlota.Infrastructure.Implementation;

public class RepuestoRepository : IRepuestoRepository
{
    private readonly MantenimientoFlotaDbContext _context;

    public RepuestoRepository(MantenimientoFlotaDbContext context)
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

    public async Task<Repuesto?> GetByIdAsync(int id)
    {
        return await _context.Repuestos
            .AsNoTracking()
            .FirstOrDefaultAsync(repuesto => repuesto.Id == id);
    }

    public async Task<RepuestoDetalleDto?> GetDetalleByIdAsync(int id)
    {
        return await _context.Repuestos
            .AsNoTracking()
            .AsSingleQuery()
            .Where(repuesto => repuesto.Id == id)
            .Select(repuesto => new RepuestoDetalleDto
            {
                Id = repuesto.Id,
                Codigo = repuesto.Codigo,
                Nombre = repuesto.Nombre,
                UnidadMedida = repuesto.UnidadMedida,
                StockActual = repuesto.StockActual,
                StockMinimo = repuesto.StockMinimo,
                StockBajo = repuesto.StockActual < repuesto.StockMinimo,
                FaltanteStockMinimo = repuesto.StockActual < repuesto.StockMinimo
                    ? repuesto.StockMinimo - repuesto.StockActual
                    : 0m,
                CantidadProveedores = repuesto.RepuestoProveedores.Count(),
                Proveedores = repuesto.RepuestoProveedores
                    .OrderBy(relacion => relacion.Proveedor.Nombre)
                    .ThenBy(relacion => relacion.ProveedorId)
                    .Select(relacion => new ProveedorRepuestoResumenDto
                    {
                        ProveedorId = relacion.ProveedorId,
                        Nombre = relacion.Proveedor.Nombre,
                        PrecioOfrecido = relacion.PrecioOfrecido,
                        TiempoEntregaDias = relacion.TiempoEntregaDias
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<Repuesto?> AddAsync(Repuesto repuesto)
    {
        await _context.Repuestos.AddAsync(repuesto);
        await _context.SaveChangesAsync();

        return repuesto;
    }

    public void Update(Repuesto repuesto)
    {
        _context.Repuestos.Update(repuesto);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var repuesto = _context.Repuestos.Find(id);

        if (repuesto is null)
        {
            return;
        }

        _context.Repuestos.Remove(repuesto);
        _context.SaveChanges();
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
