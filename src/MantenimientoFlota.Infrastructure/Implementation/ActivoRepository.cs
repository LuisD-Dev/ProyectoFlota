using MantenimientoFlota.Application.Interfaces.Repositories;
using MantenimientoFlota.Application.DTOs.Activo;
using MantenimientoFlota.Domain.Entities;
using MantenimientoFlota.Domain.Enums;
using MantenimientoFlota.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using MantenimientoFlota.Application.Common.Results;

namespace MantenimientoFlota.Infrastructure.Implementation;

public class ActivoRepository : IActivoRepository
{
    private readonly MantenimientoFlotaDbContext _context;

    public ActivoRepository(MantenimientoFlotaDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Activo>> GetAllAsync(
        int? categoriaActivoId = null,
        EstadoActivo? estado = null)
    {
        IQueryable<Activo> query = _context.Activos
            .AsNoTracking()
            .Include(activo => activo.CategoriaActivo);

        if (categoriaActivoId.HasValue)
        {
            query = query.Where(activo => activo.CategoriaActivoId == categoriaActivoId.Value);
        }

        if (estado.HasValue)
        {
            query = query.Where(activo => activo.Estado == estado.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<Activo?> GetByIdAsync(int id)
    {
        return await _context.Activos
            .AsNoTracking()
            .Include(activo => activo.CategoriaActivo)
            .FirstOrDefaultAsync(activo => activo.Id == id);
    }

    public async Task<ActivoDetalleDto?> GetDetalleByIdAsync(int id)
    {
        return await _context.Activos
            .AsNoTracking()
            .AsSingleQuery()
            .Where(activo => activo.Id == id)
            .Select(activo => new ActivoDetalleDto
            {
                Id = activo.Id,
                Codigo = activo.Codigo,
                Nombre = activo.Nombre,
                Descripcion = activo.Descripcion,
                FechaAdquisicion = activo.FechaAdquisicion,
                ValorReferencia = activo.ValorReferencia,
                Estado = activo.Estado,
                ImagenUrl = activo.ImagenUrl,
                Placa = activo.Placa,
                KilometrajeActual = activo.KilometrajeActual,
                CategoriaActivoId = activo.CategoriaActivoId,
                CategoriaActivoNombre = activo.CategoriaActivo.Nombre,
                Categoria = new CategoriaActivoResumenDto
                {
                    Id = activo.CategoriaActivo.Id,
                    Nombre = activo.CategoriaActivo.Nombre,
                    IntervaloMantenimientoKm = activo.CategoriaActivo.IntervaloMantenimientoKm
                },
                CantidadOrdenes = activo.OrdenesMantenimiento.Count(),
                Ordenes = activo.OrdenesMantenimiento
                    .OrderByDescending(orden => orden.FechaSolicitud)
                    .ThenByDescending(orden => orden.Id)
                    .Select(orden => new OrdenMantenimientoResumenDto
                    {
                        Codigo = orden.Codigo,
                        FechaSolicitud = orden.FechaSolicitud,
                        TipoMantenimiento = orden.TipoMantenimiento,
                        Prioridad = orden.Prioridad,
                        EstadoActual = orden.EstadoActual
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();
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

    public void UpdateEstado(Activo activo)
    {
        _context.Entry(activo).Property(entidad => entidad.Estado).IsModified = true;
        _context.SaveChanges();
    }

    public async Task ActualizarImagenUrlAsync(int id, string imagenUrl, CancellationToken cancellationToken)
    {
        var activo = new Activo { Id = id, ImagenUrl = imagenUrl };
        _context.Entry(activo).Property(entidad => entidad.ImagenUrl).IsModified = true;
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sql
            && sql.Number is 515 or 547 or 2601 or 2627 or 2628 or 8152)
        {
            throw new ImagenUpdateRejectedException(ex);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ImagenUpdateRejectedException(ex);
        }
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

    public async Task<bool> HasOpenMaintenanceOrdersAsync(int activoId)
    {
        return await _context.OrdenesMantenimiento
            .AsNoTracking()
            .AnyAsync(orden => orden.ActivoId == activoId
                && (orden.EstadoActual == EstadoOrdenMantenimiento.Solicitada
                    || orden.EstadoActual == EstadoOrdenMantenimiento.Diagnosticada
                    || orden.EstadoActual == EstadoOrdenMantenimiento.Aprobada
                    || orden.EstadoActual == EstadoOrdenMantenimiento.EnEjecucion
                    || orden.EstadoActual == EstadoOrdenMantenimiento.EnEsperaDeRepuesto));
    }

    public async Task<bool> HasAssociatedMaintenanceOrdersAsync(int activoId)
    {
        return await _context.OrdenesMantenimiento
            .AsNoTracking()
            .AnyAsync(orden => orden.ActivoId == activoId);
    }
}
