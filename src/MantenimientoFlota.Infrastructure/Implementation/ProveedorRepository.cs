using MantenimientoFlota.Application.Interfaces;
using MantenimientoFlota.Domain.Entities;
using MantenimientoFlota.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MantenimientoFlota.Infrastructure.Implementation;

public class ProveedorRepository : IProveedorRepository
{
    private readonly MantenimientoFlotaDbContext _context;

    public ProveedorRepository(MantenimientoFlotaDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Proveedor>> GetAllAsync()
    {
        return await _context.Proveedores
            .AsNoTracking()
            .OrderBy(proveedor => proveedor.Nombre)
            .ToListAsync();
    }

    public async Task<Proveedor?> GetByIdAsync(int id)
    {
        return await _context.Proveedores
            .AsNoTracking()
            .FirstOrDefaultAsync(proveedor => proveedor.Id == id);
    }

    public async Task<Proveedor> AddAsync(Proveedor proveedor)
    {
        await _context.Proveedores.AddAsync(proveedor);
        await _context.SaveChangesAsync();

        return proveedor;
    }

    public async Task UpdateAsync(Proveedor proveedor)
    {
        _context.Proveedores.Update(proveedor);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Proveedor proveedor)
    {
        _context.Proveedores.Remove(proveedor);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> HasAssociatedPartsAsync(int proveedorId)
    {
        return await _context.RepuestoProveedores
            .AsNoTracking()
            .Where(repuestoProveedor =>
                repuestoProveedor.ProveedorId == proveedorId)
            .AnyAsync();
    }
}
