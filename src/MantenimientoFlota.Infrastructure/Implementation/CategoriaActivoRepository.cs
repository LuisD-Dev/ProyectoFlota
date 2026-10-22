using MantenimientoFlota.Infrastructure.Interfaces;
using MantenimientoFlota.Domain.Entities;
using MantenimientoFlota.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MantenimientoFlota.Infrastructure.Implementation;

public class CategoriaActivoRepository : ICategoriaActivoRepository
{
    private readonly AppDbContext _context;

    public CategoriaActivoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CategoriaActivo>> GetAllAsync()
    {
        return await _context.CategoriasActivo
            .AsNoTracking()
            .ToListAsync();
    }

    public Task<CategoriaActivo?> GetForUpdateAsync(int id) =>
        _context.CategoriasActivo.FirstOrDefaultAsync(entidad => entidad.Id == id);

    public async Task<CategoriaActivo?> GetByIdAsync(int id)
    {
        return await _context.CategoriasActivo
            .AsNoTracking()
            .FirstOrDefaultAsync(categoria => categoria.Id == id);
    }

    public async Task<CategoriaActivo?> GetDetalleByIdAsync(int id)
    {
        return await _context.CategoriasActivo
            .AsNoTracking()
            .AsSingleQuery()
            .Include(entidad => entidad.Activos)
            .FirstOrDefaultAsync(entidad => entidad.Id == id);
    }

    public async Task<CategoriaActivo?> AddAsync(CategoriaActivo categoria)
    {
        await _context.CategoriasActivo.AddAsync(categoria);
        await _context.SaveChangesAsync();

        return categoria;
    }

    public async Task UpdateAsync(CategoriaActivo categoria)
    {
        if (_context.Entry(categoria).State == EntityState.Detached)
            _context.CategoriasActivo.Update(categoria);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var categoria = await _context.CategoriasActivo.FindAsync(id);

        if (categoria is null)
        {
            return;
        }

        _context.CategoriasActivo.Remove(categoria);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsByNameAsync(
        string nombre,
        int? idExcluir = null)
    {
        var query = _context.CategoriasActivo
            .AsNoTracking()
            .Where(categoria => categoria.Nombre == nombre);

        if (idExcluir.HasValue)
        {
            query = query.Where(categoria => categoria.Id != idExcluir.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<bool> HasAssociatedAssetsAsync(int categoriaActivoId)
    {
        return await _context.Activos
            .AsNoTracking()
            .AnyAsync(activo =>
                activo.CategoriaActivoId == categoriaActivoId);
    }
}
