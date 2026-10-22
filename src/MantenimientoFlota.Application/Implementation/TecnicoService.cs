using Microsoft.Extensions.Logging;
using MantenimientoFlota.Domain.DTO.PatternResult;
using MapsterMapper;
using MantenimientoFlota.Application.Mapper;
using MantenimientoFlota.Domain.DTO.Tecnico;
using MantenimientoFlota.Application.Interfaces;
using MantenimientoFlota.Infrastructure.Interfaces;
using MantenimientoFlota.Domain.Entities;

namespace MantenimientoFlota.Application.Implementation;

public class TecnicoService : ITecnicoService
{
    private readonly ITecnicoRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<TecnicoService> _logger;

    public TecnicoService(ITecnicoRepository repository, IMapper mapper, ILogger<TecnicoService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<TecnicoDto>>> GetAllAsync(
        bool? disponible = null,
        int? especialidadId = null)
    {
        if (especialidadId is <= 0)
            return Result<IEnumerable<TecnicoDto>>.Failure(400, "Solicitud no válida", "El identificador de la especialidad debe ser mayor que cero.");

        var tecnicos = await _repository.GetAllAsync(disponible, especialidadId);

        _logger.LogDebug("Operación de negocio completada. Operation={Operation}", "Tecnico.GetAllAsync");

        return Result<IEnumerable<TecnicoDto>>.Success(_mapper.Map<List<TecnicoDto>>(tecnicos), 200);
    }

    public async Task<Result<TecnicoDetalleDto>> GetByIdAsync(int id)
    {
        if (id <= 0)
            return Result<TecnicoDetalleDto>.Failure(400, "Solicitud no válida", "El identificador debe ser mayor que cero.");
        var fechaReferencia = DateOnly.FromDateTime(DateTime.UtcNow);
        var tecnico = await _repository.GetDetalleByIdAsync(id);

        if (tecnico is null)
        {
            return Result<TecnicoDetalleDto>.Failure(404, "Recurso no encontrado", $"No se encontró el técnico con id {id}.");
        }

        var detalle = _mapper.Map<TecnicoDetalleDto>(tecnico);
        foreach (var especialidad in detalle.Especialidades)
        {
            especialidad.AntiguedadAnios = MapsterConfig.CalcularAntiguedadAnios(
                especialidad.FechaObtencion, fechaReferencia);
        }

        _logger.LogDebug("Operación de negocio completada. Operation={Operation}", "Tecnico.GetByIdAsync");

        return Result<TecnicoDetalleDto>.Success(detalle, 200);
    }

    public async Task<Result<TecnicoDto>> CreateAsync(CrearTecnicoDto dto)
    {
        var tecnico = _mapper.Map<Tecnico>(dto);

        var tecnicoCreado = await _repository.AddAsync(tecnico)
            ?? tecnico;

        _logger.LogInformation("Operación de negocio completada. Operation={Operation}", "Tecnico.CreateAsync");

        return Result<TecnicoDto>.Success(_mapper.Map<TecnicoDto>(tecnicoCreado), 201);
    }

    public async Task<Result<object?>> UpdateAsync(
        int id,
        ActualizarTecnicoDto dto)
    {
        if (id <= 0)
            return Result<object?>.Failure(400, "Solicitud no válida", "El identificador debe ser mayor que cero.");
        var tecnico = await _repository.GetForUpdateAsync(id);

        if (tecnico is null)
        {
            return Result<object?>.Failure(404, "Recurso no encontrado", $"No se encontró el técnico con id {id}.");
        }

        _mapper.Map(dto, tecnico);

        await _repository.UpdateAsync(tecnico);

        _logger.LogInformation("Operación de negocio completada. Operation={Operation}", "Tecnico.UpdateAsync");

        return Result<object?>.Success(null, 204);
    }

    public async Task<Result<object?>> DeleteAsync(int id)
    {
        if (id <= 0)
            return Result<object?>.Failure(400, "Solicitud no válida", "El identificador debe ser mayor que cero.");
        var tecnico = await _repository.GetForUpdateAsync(id);

        if (tecnico is null)
        {
            return Result<object?>.Failure(404, "Recurso no encontrado", $"No se encontró el técnico con id {id}.");
        }

        if (await _repository.HasAssociatedSpecialtiesAsync(id))
        {
            return Result<object?>.Failure(409, "Conflicto", "No se puede eliminar el técnico porque tiene especialidades asociadas.");
        }

        await _repository.DeleteAsync(id);

        _logger.LogInformation("Operación de negocio completada. Operation={Operation}", "Tecnico.DeleteAsync");

        return Result<object?>.Success(null, 204);
    }

}
