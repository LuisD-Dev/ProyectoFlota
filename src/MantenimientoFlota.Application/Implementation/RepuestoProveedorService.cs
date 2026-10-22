using MantenimientoFlota.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using MantenimientoFlota.Domain.DTO.PatternResult;
using MapsterMapper;
using MantenimientoFlota.Application.Mapper;
using MantenimientoFlota.Domain.DTO.RepuestoProveedor;
using MantenimientoFlota.Application.Interfaces;
using MantenimientoFlota.Infrastructure.Interfaces;
using MantenimientoFlota.Domain.Entities;

namespace MantenimientoFlota.Application.Implementation;

public class RepuestoProveedorService : IRepuestoProveedorService
{
    private readonly IRepuestoProveedorRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<RepuestoProveedorService> _logger;

    public RepuestoProveedorService(
        IRepuestoProveedorRepository repository, IMapper mapper, ILogger<RepuestoProveedorService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<RepuestoProveedorDto>>> GetAllAsync(int repuestoId)
    {
        if (repuestoId <= 0) throw new InvalidRequestException("El identificador debe ser mayor que cero.");
        if (!await _repository.RepuestoExistsAsync(repuestoId))
            throw new ResourceNotFoundException($"No se encontró el repuesto con id {repuestoId}.");
        var repuestoProveedores = await _repository.GetAllAsync(repuestoId);

        _logger.LogDebug("Operación de negocio completada. Operation={Operation}", "RepuestoProveedor.GetAllAsync");

        return Result<IReadOnlyList<RepuestoProveedorDto>>.Success(_mapper.Map<List<RepuestoProveedorDto>>(repuestoProveedores), 200);
    }

    public async Task<Result<RepuestoProveedorDto>> GetByIdAsync(
        int repuestoId,
        int proveedorId)
    {
        if (repuestoId <= 0 || proveedorId <= 0)
            throw new InvalidRequestException("Los identificadores de repuesto y proveedor deben ser mayores que cero.");
        var repuestoProveedor = await _repository.GetByIdAsync(
            repuestoId,
            proveedorId);

        if (repuestoProveedor is null) throw new ResourceNotFoundException($"No se encontró la relación entre el repuesto con id {repuestoId} y el proveedor con id {proveedorId}.");
        _logger.LogDebug("Operación de negocio completada. Operation={Operation}", "RepuestoProveedor.GetByIdAsync");
        return Result<RepuestoProveedorDto>.Success(_mapper.Map<RepuestoProveedorDto>(repuestoProveedor), 200);
    }

    public async Task<Result<RepuestoProveedorDto>> CreateAsync(
        int repuestoId,
        CrearRepuestoProveedorDto dto)
    {
        if (repuestoId <= 0) throw new InvalidRequestException("El identificador debe ser mayor que cero.");
        var proveedorId = dto.ProveedorId!.Value;

        if (!await _repository.RepuestoExistsAsync(repuestoId))
        {
            throw new ResourceNotFoundException($"No se encontró el repuesto con id {repuestoId}.");
        }

        if (!await _repository.ProveedorExistsAsync(proveedorId))
        {
            throw new ResourceNotFoundException($"No se encontró el proveedor con id {dto.ProveedorId}.");
        }

        if (await _repository.ExistsAsync(repuestoId, proveedorId))
        {
            throw new ConflictException("Ya existe una relación entre el repuesto y el proveedor indicados.");
        }

        var repuestoProveedor = _mapper.Map<RepuestoProveedor>(dto);
        repuestoProveedor.RepuestoId = repuestoId;
        repuestoProveedor.ProveedorId = proveedorId;

        var relacionCreada = await _repository.AddAsync(repuestoProveedor);
        var relacionCompleta = await _repository.GetByIdAsync(
            relacionCreada.RepuestoId,
            relacionCreada.ProveedorId)
            ?? relacionCreada;

        _logger.LogInformation("Operación de negocio completada. Operation={Operation}", "RepuestoProveedor.CreateAsync");

        return Result<RepuestoProveedorDto>.Success(_mapper.Map<RepuestoProveedorDto>(relacionCompleta), 201);
    }

    public async Task<Result<object?>> UpdateAsync(
        int repuestoId,
        int proveedorId,
        ActualizarRepuestoProveedorDto dto)
    {
        if (repuestoId <= 0 || proveedorId <= 0)
            throw new InvalidRequestException("Los identificadores de repuesto y proveedor deben ser mayores que cero.");
        var repuestoProveedor = await _repository.GetForUpdateAsync(repuestoId, proveedorId);
        if (repuestoProveedor is null)
        {
            throw new ResourceNotFoundException($"No se encontró la relación entre el repuesto con id {repuestoId} y el proveedor con id {proveedorId}.");
        }

        _mapper.Map(dto, repuestoProveedor);
        repuestoProveedor.RepuestoId = repuestoId;
        repuestoProveedor.ProveedorId = proveedorId;

        await _repository.UpdateAsync(repuestoProveedor);

        _logger.LogInformation("Operación de negocio completada. Operation={Operation}", "RepuestoProveedor.UpdateAsync");

        return Result<object?>.Success(null, 204);
    }

    public async Task<Result<object?>> DeleteAsync(
        int repuestoId,
        int proveedorId)
    {
        if (repuestoId <= 0 || proveedorId <= 0)
            throw new InvalidRequestException("Los identificadores de repuesto y proveedor deben ser mayores que cero.");
        if (!await _repository.ExistsAsync(repuestoId, proveedorId))
        {
            throw new ResourceNotFoundException($"No se encontró la relación entre el repuesto con id {repuestoId} y el proveedor con id {proveedorId}.");
        }

        await _repository.DeleteAsync(repuestoId, proveedorId);

        _logger.LogInformation("Operación de negocio completada. Operation={Operation}", "RepuestoProveedor.DeleteAsync");

        return Result<object?>.Success(null, 204);
    }

}
