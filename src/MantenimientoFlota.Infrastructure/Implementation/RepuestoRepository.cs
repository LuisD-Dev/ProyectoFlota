using MantenimientoFlota.Application.Interfaces;
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

    public async Task<IEnumerable<Repuesto>> GetAllAsync()
    {
        return await _context.Repuestos
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Repuesto?> GetByIdAsync(int id)
    {
        return await _context.Repuestos
            .AsNoTracking()
            .FirstOrDefaultAsync(repuesto => repuesto.Id == id);
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

    public async Task<bool> HasAssociatedSuppliersAsync(int repuestoId)
    {
        return await _context.RepuestoProveedores
            .AsNoTracking()
            .AnyAsync(repuestoProveedor => repuestoProveedor.RepuestoId == repuestoId);
    }
}
