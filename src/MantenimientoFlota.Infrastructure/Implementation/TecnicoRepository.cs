using MantenimientoFlota.Domain.DTO.Tecnico;
using MantenimientoFlota.Infrastructure.Interfaces;
using MantenimientoFlota.Domain.Entities;
using MantenimientoFlota.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MantenimientoFlota.Infrastructure.Implementation;

public class TecnicoRepository : ITecnicoRepository
{
    private readonly AppDbContext _context;

    public TecnicoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Tecnico>> GetAllAsync(
        bool? disponible = null,
        int? especialidadId = null)
    {
        IQueryable<Tecnico> query = _context.Tecnicos.AsNoTracking();

        if (disponible.HasValue)
        {
            query = query.Where(tecnico => tecnico.Disponible == disponible.Value);
        }

        if (especialidadId.HasValue)
        {
            query = query.Where(tecnico =>
                tecnico.TecnicoEspecialidades.Any(relacion =>
                    relacion.EspecialidadId == especialidadId.Value));
        }

        return await query
            .OrderBy(tecnico => tecnico.NombreCompleto)
            .ToListAsync();
    }

    public Task<Tecnico?> GetForUpdateAsync(int id) =>
        _context.Tecnicos.FirstOrDefaultAsync(entidad => entidad.Id == id);

    public async Task<Tecnico?> GetByIdAsync(int id)
    {
        return await _context.Tecnicos
            .AsNoTracking()
            .FirstOrDefaultAsync(tecnico => tecnico.Id == id);
    }

    public async Task<TecnicoDetalleDto?> GetDetalleByIdAsync(int id)
    {
        // El conteo se traduce a COUNT en SQL; las relaciones se proyectan a resúmenes.
        return await _context.Tecnicos
            .AsNoTracking()
            .AsSingleQuery()
            .Where(entidad => entidad.Id == id)
            .Select(entidad => new TecnicoDetalleDto
            {
                Id = entidad.Id, NombreCompleto = entidad.NombreCompleto, Identificacion = entidad.Identificacion,
                Telefono = entidad.Telefono, Correo = entidad.Correo, Disponible = entidad.Disponible,
                TarifaPorHora = entidad.TarifaPorHora,
                CantidadEspecialidades = entidad.TecnicoEspecialidades.Count(),
                Especialidades = entidad.TecnicoEspecialidades.OrderBy(relacion => relacion.Especialidad.Nombre)
                    .ThenBy(relacion => relacion.EspecialidadId).Select(relacion => new EspecialidadTecnicoResumenDto {
                        EspecialidadId = relacion.EspecialidadId, Nombre = relacion.Especialidad.Nombre,
                        NivelCertificacion = relacion.NivelCertificacion, FechaObtencion = relacion.FechaObtencion }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<Tecnico?> AddAsync(Tecnico tecnico)
    {
        await _context.Tecnicos.AddAsync(tecnico);
        await _context.SaveChangesAsync();

        return tecnico;
    }

    public async Task UpdateAsync(Tecnico tecnico)
    {
        if (_context.Entry(tecnico).State == EntityState.Detached)
            _context.Tecnicos.Update(tecnico);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var tecnico = await _context.Tecnicos.FindAsync(id);

        if (tecnico is null)
        {
            return;
        }

        _context.Tecnicos.Remove(tecnico);
        await _context.SaveChangesAsync();
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
