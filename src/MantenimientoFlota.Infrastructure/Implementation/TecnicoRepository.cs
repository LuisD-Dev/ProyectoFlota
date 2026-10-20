using MantenimientoFlota.Application.Interfaces.Repositories;
using MantenimientoFlota.Application.DTOs.Tecnico;
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

    public async Task<Tecnico?> GetByIdAsync(int id)
    {
        return await _context.Tecnicos
            .AsNoTracking()
            .FirstOrDefaultAsync(tecnico => tecnico.Id == id);
    }

    public async Task<TecnicoDetalleDto?> GetDetalleByIdAsync(int id)
    {
        return await _context.Tecnicos
            .AsNoTracking()
            .AsSingleQuery()
            .Where(tecnico => tecnico.Id == id)
            .Select(tecnico => new TecnicoDetalleDto
            {
                Id = tecnico.Id,
                NombreCompleto = tecnico.NombreCompleto,
                Identificacion = tecnico.Identificacion,
                Telefono = tecnico.Telefono,
                Correo = tecnico.Correo,
                Disponible = tecnico.Disponible,
                TarifaPorHora = tecnico.TarifaPorHora,
                CantidadEspecialidades = tecnico.TecnicoEspecialidades.Count(),
                Especialidades = tecnico.TecnicoEspecialidades
                    .OrderBy(relacion => relacion.Especialidad.Nombre)
                    .ThenBy(relacion => relacion.EspecialidadId)
                    .Select(relacion => new EspecialidadTecnicoResumenDto
                    {
                        EspecialidadId = relacion.EspecialidadId,
                        Nombre = relacion.Especialidad.Nombre,
                        NivelCertificacion = relacion.NivelCertificacion,
                        FechaObtencion = relacion.FechaObtencion
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();
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
