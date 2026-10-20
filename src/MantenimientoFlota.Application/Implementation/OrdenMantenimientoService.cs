using MantenimientoFlota.Application.Common.Exceptions;
using MantenimientoFlota.Application.DTOs.OrdenMantenimiento;
using MantenimientoFlota.Application.Interfaces.Services;
using MantenimientoFlota.Application.Interfaces.Repositories;
using MantenimientoFlota.Domain.Entities;
using MantenimientoFlota.Domain.Enums;

namespace MantenimientoFlota.Application.Implementation;

public class OrdenMantenimientoService : IOrdenMantenimientoService
{
    private readonly IOrdenMantenimientoRepository _repository;

    public OrdenMantenimientoService(
        IOrdenMantenimientoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<OrdenMantenimientoDto>> GetAllAsync(int? activoId = null)
    {
        if (activoId is <= 0)
            throw new InvalidRequestException("El identificador del activo debe ser mayor que cero.");
        return await _repository.GetAllAsync(activoId);
    }

    public async Task<OrdenMantenimientoDetalleDto> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new InvalidRequestException("El identificador debe ser mayor que cero.");
        var fechaReferencia = DateOnly.FromDateTime(DateTime.UtcNow);
        var orden = await _repository.GetDetalleByIdAsync(id);

        if (orden is null)
        {
            throw new ResourceNotFoundException($"No se encontró la orden de mantenimiento con id {id}.");
        }

        // Los componentes almacenados se interpretan como calendario UTC, sin conversión de zona.
        var fechaSolicitud = DateOnly.FromDateTime(orden.FechaSolicitud);
        orden.DiasTranscurridosDesdeSolicitud = Math.Max(
            0, fechaReferencia.DayNumber - fechaSolicitud.DayNumber);
        return orden;
    }

    public async Task<OrdenMantenimientoDto> CreateAsync(
        CrearOrdenMantenimientoDto dto)
    {
        var activoId = dto.ActivoId!.Value;

        if (!await _repository.ActivoExistsAsync(activoId))
        {
            throw new ResourceNotFoundException($"No se encontró el activo con id {dto.ActivoId}.");
        }

        var orden = new OrdenMantenimiento
        {
            Codigo = $"OM-{Guid.NewGuid():N}",
            ActivoId = activoId,
            Motivo = dto.Motivo.Trim(),
            TipoMantenimiento = dto.TipoMantenimiento.Trim(),
            FechaSolicitud = DateTime.UtcNow,
            FechaProgramada = null,
            FechaCierre = null,
            Prioridad = dto.Prioridad.Trim(),
            CostoTotal = 0m,
            EstadoActual = EstadoOrdenMantenimiento.Solicitada
        };

        var ordenCreada = await _repository.AddAsync(orden);
        var ordenCompleta = await _repository.GetByIdAsync(ordenCreada.Id)
            ?? MapToDto(ordenCreada);

        return ordenCompleta;
    }

    private static OrdenMantenimientoDto MapToDto(
        OrdenMantenimiento ordenMantenimiento)
    {
        return new OrdenMantenimientoDto
        {
            Id = ordenMantenimiento.Id,
            Codigo = ordenMantenimiento.Codigo,
            ActivoId = ordenMantenimiento.ActivoId,
            ActivoCodigo = ordenMantenimiento.Activo?.Codigo
                ?? string.Empty,
            ActivoNombre = ordenMantenimiento.Activo?.Nombre
                ?? string.Empty,
            Motivo = ordenMantenimiento.Motivo,
            TipoMantenimiento = ordenMantenimiento.TipoMantenimiento,
            FechaSolicitud = ordenMantenimiento.FechaSolicitud,
            FechaProgramada = ordenMantenimiento.FechaProgramada,
            FechaCierre = ordenMantenimiento.FechaCierre,
            Prioridad = ordenMantenimiento.Prioridad,
            CostoTotal = ordenMantenimiento.CostoTotal,
            EstadoActual = ordenMantenimiento.EstadoActual
        };
    }
}
