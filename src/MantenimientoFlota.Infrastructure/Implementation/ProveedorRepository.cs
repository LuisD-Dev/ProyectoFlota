using MantenimientoFlota.Application.Interfaces.Repositories;
using MantenimientoFlota.Application.DTOs.Proveedor;
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

    public async Task<ProveedorDetalleDto?> GetDetalleByIdAsync(int id)
    {
        return await _context.Proveedores
            .AsNoTracking()
            .AsSingleQuery()
            .Where(proveedor => proveedor.Id == id)
            .Select(proveedor => new ProveedorDetalleDto
            {
                Id = proveedor.Id,
                Nombre = proveedor.Nombre,
                Contacto = proveedor.Contacto,
                Direccion = proveedor.Direccion,
                CondicionesGenerales = proveedor.CondicionesGenerales,
                Repuestos = proveedor.RepuestoProveedores
                    .OrderBy(relacion => relacion.Repuesto.Codigo)
                    .ThenBy(relacion => relacion.RepuestoId)
                    .Select(relacion => new RepuestoProveedorResumenDto
                    {
                        RepuestoId = relacion.RepuestoId,
                        Codigo = relacion.Repuesto.Codigo,
                        Nombre = relacion.Repuesto.Nombre,
                        PrecioOfrecido = relacion.PrecioOfrecido,
                        TiempoEntregaDias = relacion.TiempoEntregaDias
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();
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
