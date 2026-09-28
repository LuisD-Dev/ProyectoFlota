using MantenimientoFlota.Application.Interfaces;
using MantenimientoFlota.Domain.Entities;
using MantenimientoFlota.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MantenimientoFlota.Infrastructure.Implementation;

public class ActivoRepository : IActivoRepository
{
    private readonly MantenimientoFlotaDbContext _context;

    public ActivoRepository(MantenimientoFlotaDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Activo>> GetAllAsync()
    {
        return await _context.Activos
            .AsNoTracking()
            .Include(activo => activo.CategoriaActivo)
            .ToListAsync();
    }

    public async Task<Activo?> GetByIdAsync(int id)
    {
        return await _context.Activos
            .AsNoTracking()
            .Include(activo => activo.CategoriaActivo)
            .FirstOrDefaultAsync(activo => activo.Id == id);
    }

    public async Task<Activo?> AddAsync(Activo activo)
    {
        await _context.Activos.AddAsync(activo);
        await _context.SaveChangesAsync();

        return activo;
    }

    public void Update(Activo activo)
    {
        _context.Activos.Update(activo);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var activo = _context.Activos.Find(id);

        if (activo is null)
        {
            return;
        }

        _context.Activos.Remove(activo);
        _context.SaveChanges();
    }

    public async Task<bool> ExistsByCodigoAsync(
        string codigo,
        int? idExcluir = null)
    {
        var query = _context.Activos
            .AsNoTracking()
            .Where(activo => activo.Codigo == codigo);

        if (idExcluir.HasValue)
        {
            query = query.Where(activo => activo.Id != idExcluir.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<bool> CategoriaExisteAsync(int categoriaActivoId)
    {
        return await _context.CategoriasActivo
            .AsNoTracking()
            .AnyAsync(categoria => categoria.Id == categoriaActivoId);
    }
}
