using MantenimientoFlota.Infrastructure.Interfaces;
using MantenimientoFlota.Domain.Entities;
using MantenimientoFlota.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MantenimientoFlota.Infrastructure.Implementation;

public sealed class RepuestoProveedorRepository : IRepuestoProveedorRepository
{
    private readonly AppDbContext _context;

    public RepuestoProveedorRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<RepuestoProveedor>> GetAllAsync(int repuestoId)
    {
        return await _context.RepuestoProveedores
            .AsNoTracking()
            .Where(relacion => relacion.RepuestoId == repuestoId)
            .Include(repuestoProveedor => repuestoProveedor.Repuesto)
            .Include(repuestoProveedor => repuestoProveedor.Proveedor)
            .OrderBy(repuestoProveedor => repuestoProveedor.Repuesto.Nombre)
            .ThenBy(repuestoProveedor => repuestoProveedor.Proveedor.Nombre)
            .ToListAsync();
    }

    public Task<RepuestoProveedor?> GetForUpdateAsync(int repuestoId, int proveedorId) =>
        _context.RepuestoProveedores.FirstOrDefaultAsync(entidad => entidad.RepuestoId == repuestoId && entidad.ProveedorId == proveedorId);

    public async Task<RepuestoProveedor?> GetByIdAsync(
        int repuestoId,
        int proveedorId)
    {
        return await _context.RepuestoProveedores
            .AsNoTracking()
            .Include(repuestoProveedor => repuestoProveedor.Repuesto)
            .Include(repuestoProveedor => repuestoProveedor.Proveedor)
            .Where(repuestoProveedor =>
                repuestoProveedor.RepuestoId == repuestoId &&
                repuestoProveedor.ProveedorId == proveedorId)
            .FirstOrDefaultAsync();
    }

    public async Task<RepuestoProveedor> AddAsync(
        RepuestoProveedor repuestoProveedor)
    {
        await _context.RepuestoProveedores.AddAsync(repuestoProveedor);
        await _context.SaveChangesAsync();

        return repuestoProveedor;
    }

    public async Task UpdateAsync(RepuestoProveedor repuestoProveedor)
    {
        if (_context.Entry(repuestoProveedor).State == EntityState.Detached)
            _context.Entry(repuestoProveedor).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int repuestoId, int proveedorId)
    {
        var repuestoProveedor = await _context.RepuestoProveedores
            .Where(repuestoProveedor =>
                repuestoProveedor.RepuestoId == repuestoId &&
                repuestoProveedor.ProveedorId == proveedorId)
            .FirstOrDefaultAsync();

        if (repuestoProveedor is null)
        {
            return;
        }

        _context.RepuestoProveedores.Remove(repuestoProveedor);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int repuestoId, int proveedorId)
    {
        return await _context.RepuestoProveedores
            .AsNoTracking()
            .Where(repuestoProveedor =>
                repuestoProveedor.RepuestoId == repuestoId &&
                repuestoProveedor.ProveedorId == proveedorId)
            .AnyAsync();
    }

    public async Task<bool> RepuestoExistsAsync(int repuestoId)
    {
        return await _context.Repuestos
            .AsNoTracking()
            .AnyAsync(repuesto => repuesto.Id == repuestoId);
    }

    public async Task<bool> ProveedorExistsAsync(int proveedorId)
    {
        return await _context.Proveedores
            .AsNoTracking()
            .AnyAsync(proveedor => proveedor.Id == proveedorId);
    }
}
