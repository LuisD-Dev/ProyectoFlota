using MantenimientoFlota.Infrastructure.Interfaces;
using MantenimientoFlota.Domain.Entities;
using MantenimientoFlota.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MantenimientoFlota.Infrastructure.Implementation;

public sealed class TecnicoEspecialidadRepository : ITecnicoEspecialidadRepository
{
    private readonly AppDbContext _context;

    public TecnicoEspecialidadRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<TecnicoEspecialidad>> GetAllAsync(int tecnicoId)
    {
        return await _context.TecnicoEspecialidades
            .AsNoTracking()
            .Where(relacion => relacion.TecnicoId == tecnicoId)
            .Include(tecnicoEspecialidad => tecnicoEspecialidad.Tecnico)
            .Include(tecnicoEspecialidad => tecnicoEspecialidad.Especialidad)
            .OrderBy(tecnicoEspecialidad => tecnicoEspecialidad.Tecnico.NombreCompleto)
            .ThenBy(tecnicoEspecialidad => tecnicoEspecialidad.Especialidad.Nombre)
            .ToListAsync();
    }

    public Task<TecnicoEspecialidad?> GetForUpdateAsync(int tecnicoId, int especialidadId) =>
        _context.TecnicoEspecialidades.FirstOrDefaultAsync(entidad => entidad.TecnicoId == tecnicoId && entidad.EspecialidadId == especialidadId);

    public async Task<TecnicoEspecialidad?> GetByIdAsync(
        int tecnicoId,
        int especialidadId)
    {
        return await _context.TecnicoEspecialidades
            .AsNoTracking()
            .Include(tecnicoEspecialidad => tecnicoEspecialidad.Tecnico)
            .Include(tecnicoEspecialidad => tecnicoEspecialidad.Especialidad)
            .Where(tecnicoEspecialidad =>
                tecnicoEspecialidad.TecnicoId == tecnicoId &&
                tecnicoEspecialidad.EspecialidadId == especialidadId)
            .FirstOrDefaultAsync();
    }

    public async Task<TecnicoEspecialidad> AddAsync(
        TecnicoEspecialidad tecnicoEspecialidad)
    {
        await _context.TecnicoEspecialidades.AddAsync(tecnicoEspecialidad);
        await _context.SaveChangesAsync();

        return tecnicoEspecialidad;
    }

    public async Task UpdateAsync(TecnicoEspecialidad tecnicoEspecialidad)
    {
        if (_context.Entry(tecnicoEspecialidad).State == EntityState.Detached)
            _context.Entry(tecnicoEspecialidad).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int tecnicoId, int especialidadId)
    {
        var tecnicoEspecialidad = await _context.TecnicoEspecialidades
            .FirstOrDefaultAsync(tecnicoEspecialidad =>
                tecnicoEspecialidad.TecnicoId == tecnicoId &&
                tecnicoEspecialidad.EspecialidadId == especialidadId);

        if (tecnicoEspecialidad is null)
        {
            return;
        }

        _context.TecnicoEspecialidades.Remove(tecnicoEspecialidad);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int tecnicoId, int especialidadId)
    {
        return await _context.TecnicoEspecialidades
            .AsNoTracking()
            .Where(tecnicoEspecialidad =>
                tecnicoEspecialidad.TecnicoId == tecnicoId &&
                tecnicoEspecialidad.EspecialidadId == especialidadId)
            .AnyAsync();
    }

    public async Task<bool> TechnicianExistsAsync(int tecnicoId)
    {
        return await _context.Tecnicos
            .AsNoTracking()
            .AnyAsync(tecnico => tecnico.Id == tecnicoId);
    }

    public async Task<bool> SpecialtyExistsAsync(int especialidadId)
    {
        return await _context.Especialidades
            .AsNoTracking()
            .AnyAsync(especialidad => especialidad.Id == especialidadId);
    }
}
