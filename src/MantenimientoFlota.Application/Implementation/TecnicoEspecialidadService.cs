using MantenimientoFlota.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using MantenimientoFlota.Domain.DTO.PatternResult;
using MapsterMapper;
using MantenimientoFlota.Application.Mapper;
using MantenimientoFlota.Domain.DTO.TecnicoEspecialidad;
using MantenimientoFlota.Application.Interfaces;
using MantenimientoFlota.Infrastructure.Interfaces;
using MantenimientoFlota.Domain.Entities;

namespace MantenimientoFlota.Application.Implementation;

public class TecnicoEspecialidadService : ITecnicoEspecialidadService
{
    private readonly ITecnicoEspecialidadRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<TecnicoEspecialidadService> _logger;

    public TecnicoEspecialidadService(
        ITecnicoEspecialidadRepository repository, IMapper mapper, ILogger<TecnicoEspecialidadService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<TecnicoEspecialidadDto>>> GetAllAsync(int tecnicoId)
    {
        if (tecnicoId <= 0) throw new InvalidRequestException("El identificador debe ser mayor que cero.");
        if (!await _repository.TechnicianExistsAsync(tecnicoId))
            throw new ResourceNotFoundException($"No se encontró el técnico con id {tecnicoId}.");
        var tecnicoEspecialidades = await _repository.GetAllAsync(tecnicoId);

        _logger.LogDebug("Operación de negocio completada. Operation={Operation}", "TecnicoEspecialidad.GetAllAsync");

        return Result<IReadOnlyList<TecnicoEspecialidadDto>>.Success(_mapper.Map<List<TecnicoEspecialidadDto>>(tecnicoEspecialidades), 200);
    }

    public async Task<Result<TecnicoEspecialidadDto>> GetByIdAsync(
        int tecnicoId,
        int especialidadId)
    {
        var tecnicoEspecialidad = await _repository.GetByIdAsync(
            tecnicoId,
            especialidadId);

        if (tecnicoEspecialidad is null) throw new ResourceNotFoundException($"No se encontró la relación entre el técnico con id {tecnicoId} y la especialidad con id {especialidadId}.");
        _logger.LogDebug("Operación de negocio completada. Operation={Operation}", "TecnicoEspecialidad.GetByIdAsync");
        return Result<TecnicoEspecialidadDto>.Success(_mapper.Map<TecnicoEspecialidadDto>(tecnicoEspecialidad), 200);
    }

    public async Task<Result<TecnicoEspecialidadDto>> CreateAsync(
        int tecnicoId,
        CrearTecnicoEspecialidadDto dto)
    {
        if (tecnicoId <= 0) throw new InvalidRequestException("El identificador debe ser mayor que cero.");
        var especialidadId = dto.EspecialidadId!.Value;

        if (!await _repository.TechnicianExistsAsync(tecnicoId))
        {
            throw new ResourceNotFoundException($"No se encontró el técnico con id {tecnicoId}.");
        }

        if (!await _repository.SpecialtyExistsAsync(especialidadId))
        {
            throw new ResourceNotFoundException($"No se encontró la especialidad con id {dto.EspecialidadId}.");
        }

        if (await _repository.ExistsAsync(tecnicoId, especialidadId))
        {
            throw new ConflictException("Ya existe una relación entre el técnico y la especialidad indicados.");
        }

        var tecnicoEspecialidad = _mapper.Map<TecnicoEspecialidad>(dto);
        tecnicoEspecialidad.TecnicoId = tecnicoId;
        tecnicoEspecialidad.EspecialidadId = especialidadId;

        var relacionCreada = await _repository.AddAsync(tecnicoEspecialidad);
        var relacionCompleta = await _repository.GetByIdAsync(
            relacionCreada.TecnicoId,
            relacionCreada.EspecialidadId)
            ?? relacionCreada;

        _logger.LogInformation("Operación de negocio completada. Operation={Operation}", "TecnicoEspecialidad.CreateAsync");

        return Result<TecnicoEspecialidadDto>.Success(_mapper.Map<TecnicoEspecialidadDto>(relacionCompleta), 201);
    }

    public async Task<Result<object?>> UpdateAsync(
        int tecnicoId,
        int especialidadId,
        ActualizarTecnicoEspecialidadDto dto)
    {
        var tecnicoEspecialidad = await _repository.GetForUpdateAsync(tecnicoId, especialidadId);
        if (tecnicoEspecialidad is null)
        {
            throw new ResourceNotFoundException($"No se encontró la relación entre el técnico con id {tecnicoId} y la especialidad con id {especialidadId}.");
        }

        _mapper.Map(dto, tecnicoEspecialidad);
        tecnicoEspecialidad.TecnicoId = tecnicoId;
        tecnicoEspecialidad.EspecialidadId = especialidadId;

        await _repository.UpdateAsync(tecnicoEspecialidad);

        _logger.LogInformation("Operación de negocio completada. Operation={Operation}", "TecnicoEspecialidad.UpdateAsync");

        return Result<object?>.Success(null, 204);
    }

    public async Task<Result<object?>> DeleteAsync(
        int tecnicoId,
        int especialidadId)
    {
        if (!await _repository.ExistsAsync(tecnicoId, especialidadId))
        {
            throw new ResourceNotFoundException($"No se encontró la relación entre el técnico con id {tecnicoId} y la especialidad con id {especialidadId}.");
        }

        await _repository.DeleteAsync(tecnicoId, especialidadId);

        _logger.LogInformation("Operación de negocio completada. Operation={Operation}", "TecnicoEspecialidad.DeleteAsync");

        return Result<object?>.Success(null, 204);
    }
}
