using Microsoft.Extensions.Logging;
using MantenimientoFlota.Domain.DTO.PatternResult;
using MapsterMapper;
using MantenimientoFlota.Application.Mapper;
using MantenimientoFlota.Domain.DTO.Repuesto;
using MantenimientoFlota.Application.Interfaces;
using MantenimientoFlota.Infrastructure.Interfaces;
using MantenimientoFlota.Domain.Entities;

namespace MantenimientoFlota.Application.Implementation;

public class RepuestoService : IRepuestoService
{
    private readonly IRepuestoRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<RepuestoService> _logger;

    public RepuestoService(IRepuestoRepository repository, IMapper mapper, ILogger<RepuestoService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<RepuestoDto>>> GetAllAsync(bool stockBajo = false)
    {
        var repuestos = await _repository.GetAllAsync(stockBajo);

        _logger.LogDebug("Operación de negocio completada. Operation={Operation}", "Repuesto.GetAllAsync");

        return Result<IReadOnlyList<RepuestoDto>>.Success(_mapper.Map<List<RepuestoDto>>(repuestos), 200);
    }

    public async Task<Result<RepuestoDetalleDto>> GetByIdAsync(int id)
    {
        if (id <= 0)
            return Result<RepuestoDetalleDto>.Failure(400, "Solicitud no válida", "El identificador debe ser mayor que cero.");
        var entidad = await _repository.GetDetalleByIdAsync(id);
        if (entidad is null) return Result<RepuestoDetalleDto>.Failure(404, "Recurso no encontrado", $"No se encontró el repuesto con id {id}.");
        _logger.LogDebug("Operación de negocio completada. Operation={Operation}", "Repuesto.GetByIdAsync");
        return Result<RepuestoDetalleDto>.Success(_mapper.Map<RepuestoDetalleDto>(entidad), 200);
    }

    public async Task<Result<RepuestoDto>> CreateAsync(
        CrearRepuestoDto dto)
    {
        var codigo = dto.Codigo.Trim();

        if (await _repository.ExistsByCodigoAsync(codigo))
        {
            return Result<RepuestoDto>.Failure(409, "Conflicto", "Ya existe un repuesto con el código indicado.");
        }

        var repuesto = _mapper.Map<Repuesto>(dto);

        var repuestoCreado = await _repository.AddAsync(repuesto) ?? repuesto;

        _logger.LogInformation("Operación de negocio completada. Operation={Operation}", "Repuesto.CreateAsync");

        return Result<RepuestoDto>.Success(_mapper.Map<RepuestoDto>(repuestoCreado), 201);
    }

    public async Task<Result<object?>> UpdateAsync(
        int id,
        ActualizarRepuestoDto dto)
    {
        if (id <= 0)
            return Result<object?>.Failure(400, "Solicitud no válida", "El identificador debe ser mayor que cero.");
        var repuesto = await _repository.GetForUpdateAsync(id);

        if (repuesto is null)
        {
            return Result<object?>.Failure(404, "Recurso no encontrado", $"No se encontró el repuesto con id {id}.");
        }

        var codigo = dto.Codigo.Trim();

        if (await _repository.ExistsByCodigoAsync(codigo, id))
        {
            return Result<object?>.Failure(409, "Conflicto", "Ya existe un repuesto con el código indicado.");
        }

        _mapper.Map(dto, repuesto);

        await _repository.UpdateAsync(repuesto);

        _logger.LogInformation("Operación de negocio completada. Operation={Operation}", "Repuesto.UpdateAsync");

        return Result<object?>.Success(null, 204);
    }

    public async Task<Result<object?>> DeleteAsync(int id)
    {
        if (id <= 0)
            return Result<object?>.Failure(400, "Solicitud no válida", "El identificador debe ser mayor que cero.");
        var repuesto = await _repository.GetForUpdateAsync(id);

        if (repuesto is null)
        {
            return Result<object?>.Failure(404, "Recurso no encontrado", $"No se encontró el repuesto con id {id}.");
        }

        if (await _repository.HasAssociatedSuppliersAsync(id))
        {
            return Result<object?>.Failure(409, "Conflicto", "No se puede eliminar el repuesto porque tiene proveedores asociados.");
        }

        await _repository.DeleteAsync(id);

        _logger.LogInformation("Operación de negocio completada. Operation={Operation}", "Repuesto.DeleteAsync");

        return Result<object?>.Success(null, 204);
    }

}
