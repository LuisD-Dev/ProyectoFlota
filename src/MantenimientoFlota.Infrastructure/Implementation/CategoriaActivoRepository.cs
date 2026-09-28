using MantenimientoFlota.Application.Interfaces;
using MantenimientoFlota.Domain.Entities;
using MantenimientoFlota.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MantenimientoFlota.Infrastructure.Implementation;

public class CategoriaActivoRepository : ICategoriaActivoRepository
{
    private readonly MantenimientoFlotaDbContext _context;

    public CategoriaActivoRepository(MantenimientoFlotaDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CategoriaActivo>> GetAllAsync()
    {
        return await _context.CategoriasActivo
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<CategoriaActivo?> GetByIdAsync(int id)
    {
        return await _context.CategoriasActivo
            .AsNoTracking()
            .FirstOrDefaultAsync(categoria => categoria.Id == id);
    }

    public async Task<CategoriaActivo?> AddAsync(CategoriaActivo categoria)
    {
        await _context.CategoriasActivo.AddAsync(categoria);
        await _context.SaveChangesAsync();

        return categoria;
    }

    public void Update(CategoriaActivo categoria)
    {
        _context.CategoriasActivo.Update(categoria);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var categoria = _context.CategoriasActivo.Find(id);

        if (categoria is null)
        {
            return;
        }

        _context.CategoriasActivo.Remove(categoria);
        _context.SaveChanges();
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
