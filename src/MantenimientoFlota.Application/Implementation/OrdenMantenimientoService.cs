using MantenimientoFlota.Application.Common.Results;
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
        return await _repository.GetAllAsync(activoId);
    }

    public async Task<OrdenMantenimientoDto?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<OrdenMantenimientoOperationResult> CreateAsync(
        CrearOrdenMantenimientoDto dto)
    {
        var activoId = dto.ActivoId!.Value;

        if (!await _repository.ActivoExistsAsync(activoId))
        {
            return new(
                OrdenMantenimientoOperationStatus.ActivoDoesNotExist);
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

        return new(
            OrdenMantenimientoOperationStatus.Succeeded,
            ordenCompleta);
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
