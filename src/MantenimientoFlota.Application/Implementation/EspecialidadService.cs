using MantenimientoFlota.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using MantenimientoFlota.Domain.DTO.PatternResult;
using MapsterMapper;
using MantenimientoFlota.Application.Mapper;
using MantenimientoFlota.Domain.DTO.Especialidad;
using MantenimientoFlota.Application.Interfaces;
using MantenimientoFlota.Infrastructure.Interfaces;
using MantenimientoFlota.Domain.Entities;

namespace MantenimientoFlota.Application.Implementation;

public class EspecialidadService : IEspecialidadService
{
    private readonly IEspecialidadRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<EspecialidadService> _logger;

    public EspecialidadService(IEspecialidadRepository repository, IMapper mapper, ILogger<EspecialidadService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<EspecialidadDto>>> GetAllAsync()
    {
        var especialidades = await _repository.GetAllAsync();

        _logger.LogDebug("Operación de negocio completada. Operation={Operation}", "Especialidad.GetAllAsync");

        return Result<IReadOnlyList<EspecialidadDto>>.Success(_mapper.Map<List<EspecialidadDto>>(especialidades), 200);
    }

    public async Task<Result<EspecialidadDetalleDto>> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new InvalidRequestException("El identificador debe ser mayor que cero.");
        var entidad = await _repository.GetDetalleByIdAsync(id);
        if (entidad is null) throw new ResourceNotFoundException($"No se encontró la especialidad con id {id}.");
        _logger.LogDebug("Operación de negocio completada. Operation={Operation}", "Especialidad.GetByIdAsync");
        return Result<EspecialidadDetalleDto>.Success(_mapper.Map<EspecialidadDetalleDto>(entidad), 200);
    }

    public async Task<Result<EspecialidadDto>> CreateAsync(
        CrearEspecialidadDto dto)
    {
        var nombre = dto.Nombre.Trim();

        if (await _repository.ExistsByNameAsync(nombre))
        {
            throw new ConflictException("Ya existe una especialidad con el nombre indicado.");
        }

        var especialidad = _mapper.Map<Especialidad>(dto);

        var especialidadCreada = await _repository.AddAsync(especialidad);

        _logger.LogInformation("Operación de negocio completada. Operation={Operation}", "Especialidad.CreateAsync");

        return Result<EspecialidadDto>.Success(_mapper.Map<EspecialidadDto>(especialidadCreada), 201);
    }

    public async Task<Result<object?>> UpdateAsync(
        int id,
        ActualizarEspecialidadDto dto)
    {
        if (id <= 0)
            throw new InvalidRequestException("El identificador debe ser mayor que cero.");
        var especialidad = await _repository.GetForUpdateAsync(id);

        if (especialidad is null)
        {
            throw new ResourceNotFoundException($"No se encontró la especialidad con id {id}.");
        }

        var nombre = dto.Nombre.Trim();

        if (await _repository.ExistsByNameAsync(nombre, id))
        {
            throw new ConflictException("Ya existe una especialidad con el nombre indicado.");
        }

        _mapper.Map(dto, especialidad);

        await _repository.UpdateAsync(especialidad);

        _logger.LogInformation("Operación de negocio completada. Operation={Operation}", "Especialidad.UpdateAsync");

        return Result<object?>.Success(null, 204);
    }

    public async Task<Result<object?>> DeleteAsync(int id)
    {
        if (id <= 0)
            throw new InvalidRequestException("El identificador debe ser mayor que cero.");
        var especialidad = await _repository.GetForUpdateAsync(id);

        if (especialidad is null)
        {
            throw new ResourceNotFoundException($"No se encontró la especialidad con id {id}.");
        }

        if (await _repository.HasAssociatedTechniciansAsync(id))
        {
            throw new ConflictException("No se puede eliminar la especialidad porque tiene técnicos asociados.");
        }

        await _repository.DeleteAsync(especialidad);

        _logger.LogInformation("Operación de negocio completada. Operation={Operation}", "Especialidad.DeleteAsync");

        return Result<object?>.Success(null, 204);
    }

}
