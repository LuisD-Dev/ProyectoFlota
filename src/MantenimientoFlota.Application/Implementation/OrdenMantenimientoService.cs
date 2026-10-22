using Microsoft.Extensions.Logging;
using MantenimientoFlota.Domain.DTO.PatternResult;
using MapsterMapper;
using MantenimientoFlota.Application.Mapper;
using MantenimientoFlota.Domain.DTO.OrdenMantenimiento;
using MantenimientoFlota.Application.Interfaces;
using MantenimientoFlota.Infrastructure.Interfaces;
using MantenimientoFlota.Domain.Entities;
using MantenimientoFlota.Domain.Constants;

namespace MantenimientoFlota.Application.Implementation;

public class OrdenMantenimientoService : IOrdenMantenimientoService
{
    private readonly IOrdenMantenimientoRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<OrdenMantenimientoService> _logger;

    public OrdenMantenimientoService(
        IOrdenMantenimientoRepository repository, IMapper mapper, ILogger<OrdenMantenimientoService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<OrdenMantenimientoDto>>> GetAllAsync(int? activoId = null)
    {
        if (activoId is <= 0)
            return Result<IReadOnlyList<OrdenMantenimientoDto>>.Failure(400, "Solicitud no válida", "El identificador del activo debe ser mayor que cero.");
        var ordenes = await _repository.GetAllAsync(activoId);
        _logger.LogDebug("Operación de negocio completada. Operation={Operation}", "OrdenMantenimiento.GetAllAsync");
        return Result<IReadOnlyList<OrdenMantenimientoDto>>.Success(_mapper.Map<List<OrdenMantenimientoDto>>(ordenes), 200);
    }

    public async Task<Result<OrdenMantenimientoDetalleDto>> GetByIdAsync(int id)
    {
        if (id <= 0)
            return Result<OrdenMantenimientoDetalleDto>.Failure(400, "Solicitud no válida", "El identificador debe ser mayor que cero.");
        var fechaReferencia = DateOnly.FromDateTime(DateTime.UtcNow);
        var orden = await _repository.GetDetalleByIdAsync(id);

        if (orden is null)
        {
            return Result<OrdenMantenimientoDetalleDto>.Failure(404, "Recurso no encontrado", $"No se encontró la orden de mantenimiento con id {id}.");
        }

        // Los componentes almacenados se interpretan como calendario UTC, sin conversión de zona.
        var detalle = _mapper.Map<OrdenMantenimientoDetalleDto>(orden);
        var fechaSolicitud = DateOnly.FromDateTime(orden.FechaSolicitud);
        detalle.DiasTranscurridosDesdeSolicitud = Math.Max(
            0, fechaReferencia.DayNumber - fechaSolicitud.DayNumber);
        _logger.LogDebug("Operación de negocio completada. Operation={Operation}", "OrdenMantenimiento.GetByIdAsync");
        return Result<OrdenMantenimientoDetalleDto>.Success(detalle, 200);
    }

    public async Task<Result<OrdenMantenimientoDto>> CreateAsync(
        CrearOrdenMantenimientoDto dto)
    {
        var activoId = dto.ActivoId!.Value;

        if (!await _repository.ActivoExistsAsync(activoId))
        {
            return Result<OrdenMantenimientoDto>.Failure(404, "Recurso no encontrado", $"No se encontró el activo con id {dto.ActivoId}.");
        }

        var orden = _mapper.Map<OrdenMantenimiento>(dto);
        orden.Codigo = $"OM-{Guid.NewGuid():N}";
        orden.FechaSolicitud = DateTime.UtcNow;
        orden.EstadoActual = EstadoOrdenMantenimiento.Solicitada;

        var ordenCreada = await _repository.AddAsync(orden);
        var ordenCompleta = await _repository.GetByIdAsync(ordenCreada.Id)
            ?? ordenCreada;

        _logger.LogInformation("Operación de negocio completada. Operation={Operation}", "OrdenMantenimiento.CreateAsync");

        return Result<OrdenMantenimientoDto>.Success(_mapper.Map<OrdenMantenimientoDto>(ordenCompleta), 201);
    }
}
