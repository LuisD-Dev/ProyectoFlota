using MantenimientoFlota.Infrastructure.Interfaces;
using MantenimientoFlota.Domain.Entities;
using MantenimientoFlota.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MantenimientoFlota.Infrastructure.Implementation;

public sealed class OrdenMantenimientoRepository : IOrdenMantenimientoRepository
{
    private readonly AppDbContext _context;

    public OrdenMantenimientoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<OrdenMantenimiento>> GetAllAsync(int? activoId = null)
    {
        IQueryable<OrdenMantenimiento> query = _context.OrdenesMantenimiento
            .AsNoTracking();

        if (activoId.HasValue)
        {
            query = query.Where(orden => orden.ActivoId == activoId.Value);
        }

        return await query.Include(orden => orden.Activo)
            .OrderByDescending(orden => orden.FechaSolicitud)
            .ThenByDescending(orden => orden.Id)
            .ToListAsync();
    }

    public async Task<OrdenMantenimiento?> GetByIdAsync(int id)
    {
        IQueryable<OrdenMantenimiento> query = _context.OrdenesMantenimiento
            .AsNoTracking()
            .Where(orden => orden.Id == id);

        return await query.Include(orden => orden.Activo).FirstOrDefaultAsync();
    }

    public async Task<OrdenMantenimiento?> GetDetalleByIdAsync(int id)
    {
        return await _context.OrdenesMantenimiento
            .AsNoTracking()
            .AsSingleQuery()
            .Include(entidad => entidad.Activo)
            .ThenInclude(activo => activo.CategoriaActivo)
            .FirstOrDefaultAsync(entidad => entidad.Id == id);
    }

    public async Task<OrdenMantenimiento> AddAsync(
        OrdenMantenimiento ordenMantenimiento)
    {
        await _context.OrdenesMantenimiento.AddAsync(ordenMantenimiento);
        await _context.SaveChangesAsync();

        return ordenMantenimiento;
    }

    public async Task<bool> ActivoExistsAsync(int activoId)
    {
        return await _context.Activos
            .AsNoTracking()
            .Where(activo => activo.Id == activoId)
            .AnyAsync();
    }
}
