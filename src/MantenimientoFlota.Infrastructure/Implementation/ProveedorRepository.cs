using MantenimientoFlota.Infrastructure.Interfaces;
using MantenimientoFlota.Domain.Entities;
using MantenimientoFlota.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MantenimientoFlota.Infrastructure.Implementation;

public class ProveedorRepository : IProveedorRepository
{
    private readonly AppDbContext _context;

    public ProveedorRepository(AppDbContext context)
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

    public Task<Proveedor?> GetForUpdateAsync(int id) =>
        _context.Proveedores.FirstOrDefaultAsync(entidad => entidad.Id == id);

    public async Task<Proveedor?> GetByIdAsync(int id)
    {
        return await _context.Proveedores
            .AsNoTracking()
            .FirstOrDefaultAsync(proveedor => proveedor.Id == id);
    }

    public async Task<Proveedor?> GetDetalleByIdAsync(int id)
    {
        return await _context.Proveedores
            .AsNoTracking()
            .AsSingleQuery()
            .Include(entidad => entidad.RepuestoProveedores)
            .ThenInclude(relacion => relacion.Repuesto)
            .FirstOrDefaultAsync(entidad => entidad.Id == id);
    }

    public async Task<Proveedor> AddAsync(Proveedor proveedor)
    {
        await _context.Proveedores.AddAsync(proveedor);
        await _context.SaveChangesAsync();

        return proveedor;
    }

    public async Task UpdateAsync(Proveedor proveedor)
    {
        if (_context.Entry(proveedor).State == EntityState.Detached)
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
