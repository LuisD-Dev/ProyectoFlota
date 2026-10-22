using MantenimientoFlota.Infrastructure.Interfaces;
using MantenimientoFlota.Domain.Entities;
using MantenimientoFlota.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MantenimientoFlota.Infrastructure.Implementation;

public class EspecialidadRepository : IEspecialidadRepository
{
    private readonly AppDbContext _context;

    public EspecialidadRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Especialidad>> GetAllAsync()
    {
        return await _context.Especialidades
            .AsNoTracking()
            .OrderBy(especialidad => especialidad.Nombre)
            .ToListAsync();
    }

    public Task<Especialidad?> GetForUpdateAsync(int id) =>
        _context.Especialidades.FirstOrDefaultAsync(entidad => entidad.Id == id);

    public async Task<Especialidad?> GetByIdAsync(int id)
    {
        return await _context.Especialidades
            .AsNoTracking()
            .FirstOrDefaultAsync(especialidad => especialidad.Id == id);
    }

    public async Task<Especialidad?> GetDetalleByIdAsync(int id)
    {
        return await _context.Especialidades
            .AsNoTracking()
            .AsSingleQuery()
            .Include(entidad => entidad.TecnicoEspecialidades)
            .ThenInclude(relacion => relacion.Tecnico)
            .FirstOrDefaultAsync(entidad => entidad.Id == id);
    }

    public async Task<Especialidad> AddAsync(Especialidad especialidad)
    {
        await _context.Especialidades.AddAsync(especialidad);
        await _context.SaveChangesAsync();

        return especialidad;
    }

    public async Task UpdateAsync(Especialidad especialidad)
    {
        if (_context.Entry(especialidad).State == EntityState.Detached)
            _context.Especialidades.Update(especialidad);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Especialidad especialidad)
    {
        _context.Especialidades.Remove(especialidad);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsByNameAsync(
        string nombre,
        int? idExcluir = null)
    {
        var query = _context.Especialidades
            .AsNoTracking()
            .Where(especialidad => especialidad.Nombre == nombre);

        if (idExcluir.HasValue)
        {
            query = query.Where(especialidad => especialidad.Id != idExcluir.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<bool> HasAssociatedTechniciansAsync(int especialidadId)
    {
        return await _context.TecnicoEspecialidades
            .AsNoTracking()
            .AnyAsync(tecnicoEspecialidad =>
                tecnicoEspecialidad.EspecialidadId == especialidadId);
    }
}
