using Microsoft.Extensions.Logging;
using MantenimientoFlota.Domain.DTO.PatternResult;
using MapsterMapper;
using MantenimientoFlota.Application.Mapper;
using MantenimientoFlota.Infrastructure.Exceptions;
using MantenimientoFlota.Domain.DTO.Activo;
using MantenimientoFlota.Application.Interfaces;
using MantenimientoFlota.Infrastructure.Interfaces;
using MantenimientoFlota.Domain.Entities;
using MantenimientoFlota.Domain.Constants;

namespace MantenimientoFlota.Application.Implementation;

public class ActivoService : IActivoService
{
    private readonly IActivoRepository _repository;
    private readonly IActivoImagenStorage _imagenes;
    private readonly IMapper _mapper;
    private readonly ILogger<ActivoService> _logger;

    public ActivoService(IActivoRepository repository, IActivoImagenStorage imagenes, IMapper mapper, ILogger<ActivoService> logger)
    {
        _repository = repository;
        _imagenes = imagenes;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<ActivoDto>>> GetAllAsync(
        int? categoriaActivoId = null,
        int? estado = null)
    {
        var activos = await _repository.GetAllAsync(categoriaActivoId, estado);

        _logger.LogDebug("Operación de negocio completada. Operation={Operation}", "Activo.GetAllAsync");

        return Result<IReadOnlyList<ActivoDto>>.Success(_mapper.Map<List<ActivoDto>>(activos), 200);
    }

    public async Task<Result<ActivoDetalleDto>> GetByIdAsync(int id)
    {
        if (id <= 0)
            return Result<ActivoDetalleDto>.Failure(400, "Solicitud no válida", "El identificador debe ser mayor que cero.");
        var fechaReferencia = DateOnly.FromDateTime(DateTime.UtcNow);
        var activo = await _repository.GetDetalleByIdAsync(id);

        if (activo is null)
        {
            return Result<ActivoDetalleDto>.Failure(404, "Recurso no encontrado", $"No se encontró el activo con id {id}.");
        }

        var detalle = _mapper.Map<ActivoDetalleDto>(activo);
        detalle.AntiguedadAnios = MapsterConfig.CalcularAntiguedadAnios(
            activo.FechaAdquisicion, fechaReferencia);
        _logger.LogDebug("Operación de negocio completada. Operation={Operation}", "Activo.GetByIdAsync");
        return Result<ActivoDetalleDto>.Success(detalle, 200);
    }

    public async Task<Result<ActivoDto>> CreateAsync(CrearActivoDto dto)
    {
        if (_imagenes.EsRutaReservada(dto.ImagenUrl))
        {
            return Result<ActivoDto>.Failure(400, "Solicitud no válida", "Las rutas de imágenes administradas solo pueden asignarse mediante la carga de imagen del activo.");
        }

        var codigo = dto.Codigo.Trim();

        if (await _repository.ExistsByCodigoAsync(codigo))
        {
            return Result<ActivoDto>.Failure(409, "Conflicto", "Ya existe un activo con el código indicado.");
        }

        var categoriaActivoId = dto.CategoriaActivoId!.Value;

        if (!await _repository.CategoriaExisteAsync(categoriaActivoId))
        {
            return Result<ActivoDto>.Failure(400, "Solicitud no válida", "No existe una categoría de activo con el identificador indicado.");
        }

        var activo = _mapper.Map<Activo>(dto);

        var activoCreado = await _repository.AddAsync(activo)
            ?? activo;

        var activoCompleto = await _repository.GetByIdAsync(activoCreado.Id)
            ?? activoCreado;

        _logger.LogInformation("Operación de negocio completada. Operation={Operation}", "Activo.CreateAsync");

        return Result<ActivoDto>.Success(_mapper.Map<ActivoDto>(activoCompleto), 201);
    }

    public async Task<Result<object?>> UpdateAsync(
        int id,
        ActualizarActivoDto dto)
    {
        if (id <= 0)
            return Result<object?>.Failure(400, "Solicitud no válida", "El identificador debe ser mayor que cero.");
        var activoExistente = await _repository.GetForUpdateAsync(id);

        if (activoExistente is null)
        {
            return Result<object?>.Failure(404, "Recurso no encontrado", $"No se encontró el activo con id {id}.");
        }

        if (_imagenes.EsRutaReservada(dto.ImagenUrl)
            && !(dto.ImagenUrl == activoExistente.ImagenUrl
                && _imagenes.EsRutaAdministrada(id, activoExistente.ImagenUrl)))
        {
            return Result<object?>.Failure(400, "Solicitud no válida", "Las rutas de imágenes administradas solo pueden asignarse mediante la carga de imagen del activo.");
        }

        var codigo = dto.Codigo.Trim();

        if (await _repository.ExistsByCodigoAsync(codigo, id))
        {
            return Result<object?>.Failure(409, "Conflicto", "Ya existe un activo con el código indicado.");
        }

        var categoriaActivoId = dto.CategoriaActivoId!.Value;

        if (!await _repository.CategoriaExisteAsync(categoriaActivoId))
        {
            return Result<object?>.Failure(400, "Solicitud no válida", "No existe una categoría de activo con el identificador indicado.");
        }

        if (activoExistente.Estado != dto.Estado!.Value
            && await TieneConflictoEstadoAdministrativoAsync(id, dto.Estado.Value))
        {
            return Result<object?>.Failure(409, "Conflicto", "No se puede cambiar el estado del activo porque tiene órdenes de mantenimiento abiertas.");
        }

        var activoActualizado = _mapper.Map(dto, activoExistente);
        activoActualizado.Id = id;
        activoActualizado.ImagenUrl = _imagenes.EsRutaAdministrada(id, activoExistente.ImagenUrl)
            ? activoExistente.ImagenUrl : dto.ImagenUrl ?? string.Empty;

        await _repository.UpdateAsync(activoActualizado);

        _logger.LogInformation("Operación de negocio completada. Operation={Operation}", "Activo.UpdateAsync");

        return Result<object?>.Success(null, 204);
    }

    public async Task<Result<object?>> CambiarEstadoAdministrativoAsync(
        int id,
        CambiarEstadoActivoDto dto)
    {
        if (id <= 0)
            return Result<object?>.Failure(400, "Solicitud no válida", "El identificador debe ser mayor que cero.");
        if (dto.Estado is not (EstadoActivo.FueraDeServicio or EstadoActivo.DadoDeBaja))
        {
            return Result<object?>.Failure(400, "Solicitud no válida", "Solo se permite establecer el estado FueraDeServicio o DadoDeBaja.");
        }

        var activo = await _repository.GetForUpdateAsync(id);
        if (activo is null)
        {
            return Result<object?>.Failure(404, "Recurso no encontrado", $"No se encontró el activo con id {id}.");
        }

        if (await TieneConflictoEstadoAdministrativoAsync(id, dto.Estado.Value))
        {
            return Result<object?>.Failure(409, "Conflicto", "No se puede cambiar el estado del activo porque tiene órdenes de mantenimiento abiertas.");
        }

        if (activo.Estado == dto.Estado.Value)
        {
            _logger.LogInformation("Operación de negocio completada. Operation={Operation}", "Activo.CambiarEstadoAdministrativoAsync");
            return Result<object?>.Success(null, 204);
        }

        activo.Estado = dto.Estado.Value;
        await _repository.UpdateEstadoAsync(activo);
        _logger.LogInformation("Operación de negocio completada. Operation={Operation}", "Activo.CambiarEstadoAdministrativoAsync");
        return Result<object?>.Success(null, 204);
    }

    private async Task<bool> TieneConflictoEstadoAdministrativoAsync(int id, int destino)
    {
        return destino is EstadoActivo.FueraDeServicio or EstadoActivo.DadoDeBaja
            && await _repository.HasOpenMaintenanceOrdersAsync(id);
    }

    public async Task<Result<object?>> DeleteAsync(int id)
    {
        if (id <= 0)
            return Result<object?>.Failure(400, "Solicitud no válida", "El identificador debe ser mayor que cero.");
        var activo = await _repository.GetForUpdateAsync(id);

        if (activo is null)
        {
            return Result<object?>.Failure(404, "Recurso no encontrado", $"No se encontró el activo con id {id}.");
        }

        if (await _repository.HasAssociatedMaintenanceOrdersAsync(id))
        {
            return Result<object?>.Failure(409, "Conflicto", "No se puede eliminar el activo porque tiene órdenes de mantenimiento asociadas.");
        }

        if (activo.Estado == EstadoActivo.DadoDeBaja)
        {
            _logger.LogInformation("Operación de negocio completada. Operation={Operation}", "Activo.DeleteAsync");
            return Result<object?>.Success(null, 204);
        }

        activo.Estado = EstadoActivo.DadoDeBaja;
        await _repository.UpdateEstadoAsync(activo);

        _logger.LogInformation("Operación de negocio completada. Operation={Operation}", "Activo.DeleteAsync");

        return Result<object?>.Success(null, 204);
    }

    public async Task<Result<ActivoImagenDto>> ActualizarImagenAsync(
        int id, Stream contenido, string extension, CancellationToken cancellationToken)
    {
        if (id <= 0)
            return Result<ActivoImagenDto>.Failure(400, "Solicitud no válida", "El identificador debe ser mayor que cero.");
        var activo = await _repository.GetByIdAsync(id);
        if (activo is null) return Result<ActivoImagenDto>.Failure(404, "Recurso no encontrado", $"No se encontró el activo con id {id}.");

        var nuevaRuta = await _imagenes.GuardarAsync(id, contenido, extension, cancellationToken);
        try
        {
            await _repository.ActualizarImagenUrlAsync(id, nuevaRuta, cancellationToken);
        }
        catch (ImagenUpdateRejectedException)
        {
            await _imagenes.EliminarSiAdministradaAsync(id, nuevaRuta);
            throw;
        }
        // Otros fallos pueden tener resultado SQL incierto: conservar ambos archivos.
        await _imagenes.EliminarSiAdministradaAsync(id, activo.ImagenUrl);
        _logger.LogInformation("Operación de negocio completada. Operation={Operation}", "Activo.ActualizarImagenAsync");
        return Result<ActivoImagenDto>.Success(new ActivoImagenDto(id, nuevaRuta), 200);
    }

}
