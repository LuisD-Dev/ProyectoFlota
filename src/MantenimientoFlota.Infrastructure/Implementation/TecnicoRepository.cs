using MantenimientoFlota.Application.Interfaces;
using MantenimientoFlota.Domain.Entities;
using MantenimientoFlota.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MantenimientoFlota.Infrastructure.Implementation;

public class TecnicoRepository : ITecnicoRepository
{
    private readonly MantenimientoFlotaDbContext _context;

    public TecnicoRepository(MantenimientoFlotaDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Tecnico>> GetAllAsync()
    {
        return await _context.Tecnicos
            .AsNoTracking()
            .OrderBy(tecnico => tecnico.NombreCompleto)
            .ToListAsync();
    }

    public async Task<Tecnico?> GetByIdAsync(int id)
    {
        return await _context.Tecnicos
            .AsNoTracking()
            .FirstOrDefaultAsync(tecnico => tecnico.Id == id);
    }

    public async Task<Tecnico?> AddAsync(Tecnico tecnico)
    {
        await _context.Tecnicos.AddAsync(tecnico);
        await _context.SaveChangesAsync();

        return tecnico;
    }

    public void Update(Tecnico tecnico)
    {
        _context.Tecnicos.Update(tecnico);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var tecnico = _context.Tecnicos.Find(id);

        if (tecnico is null)
        {
            return;
        }

        _context.Tecnicos.Remove(tecnico);
        _context.SaveChanges();
    }

    public async Task<bool> HasAssociatedSpecialtiesAsync(int tecnicoId)
    {
        return await _context.TecnicoEspecialidades
            .AsNoTracking()
            .Where(tecnicoEspecialidad =>
                tecnicoEspecialidad.TecnicoId == tecnicoId)
            .AnyAsync();
    }
}
