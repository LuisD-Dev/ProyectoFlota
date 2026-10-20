using MantenimientoFlota.Application.Interfaces.Repositories;
using MantenimientoFlota.Application.DTOs.OrdenMantenimiento;
using MantenimientoFlota.Domain.Entities;
using MantenimientoFlota.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MantenimientoFlota.Infrastructure.Implementation;

public sealed class OrdenMantenimientoRepository : IOrdenMantenimientoRepository
{
    private readonly MantenimientoFlotaDbContext _context;

    public OrdenMantenimientoRepository(MantenimientoFlotaDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<OrdenMantenimientoDto>> GetAllAsync(int? activoId = null)
    {
        IQueryable<OrdenMantenimiento> query = _context.OrdenesMantenimiento
            .AsNoTracking();

        if (activoId.HasValue)
        {
            query = query.Where(orden => orden.ActivoId == activoId.Value);
        }

        return await ProjectToDto(query)
            .OrderByDescending(orden => orden.FechaSolicitud)
            .ThenByDescending(orden => orden.Id)
            .ToListAsync();
    }

    public async Task<OrdenMantenimientoDto?> GetByIdAsync(int id)
    {
        IQueryable<OrdenMantenimiento> query = _context.OrdenesMantenimiento
            .AsNoTracking()
            .Where(orden => orden.Id == id);

        return await ProjectToDto(query).FirstOrDefaultAsync();
    }

    public async Task<OrdenMantenimientoDetalleDto?> GetDetalleByIdAsync(int id)
    {
        return await _context.OrdenesMantenimiento
            .AsNoTracking()
            .Where(orden => orden.Id == id)
            .Select(orden => new OrdenMantenimientoDetalleDto
            {
                Id = orden.Id,
                Codigo = orden.Codigo,
                ActivoId = orden.ActivoId,
                ActivoCodigo = orden.Activo.Codigo,
                ActivoNombre = orden.Activo.Nombre,
                Motivo = orden.Motivo,
                TipoMantenimiento = orden.TipoMantenimiento,
                FechaSolicitud = orden.FechaSolicitud,
                FechaProgramada = orden.FechaProgramada,
                FechaCierre = orden.FechaCierre,
                Prioridad = orden.Prioridad,
                CostoTotal = orden.CostoTotal,
                EstadoActual = orden.EstadoActual,
                Activo = new ActivoOrdenResumenDto
                {
                    Id = orden.Activo.Id,
                    Codigo = orden.Activo.Codigo,
                    Nombre = orden.Activo.Nombre,
                    Estado = orden.Activo.Estado,
                    Categoria = new CategoriaOrdenResumenDto
                    {
                        Id = orden.Activo.CategoriaActivo.Id,
                        Nombre = orden.Activo.CategoriaActivo.Nombre
                    }
                }
            })
            .FirstOrDefaultAsync();
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

    private static IQueryable<OrdenMantenimientoDto> ProjectToDto(
        IQueryable<OrdenMantenimiento> query)
    {
        return query.Select(orden => new OrdenMantenimientoDto
        {
            Id = orden.Id,
            Codigo = orden.Codigo,
            ActivoId = orden.ActivoId,
            ActivoCodigo = orden.Activo.Codigo,
            ActivoNombre = orden.Activo.Nombre,
            Motivo = orden.Motivo,
            TipoMantenimiento = orden.TipoMantenimiento,
            FechaSolicitud = orden.FechaSolicitud,
            FechaProgramada = orden.FechaProgramada,
            FechaCierre = orden.FechaCierre,
            Prioridad = orden.Prioridad,
            CostoTotal = orden.CostoTotal,
            EstadoActual = orden.EstadoActual
        });
    }
}
