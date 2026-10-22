using MantenimientoFlota.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using MantenimientoFlota.Domain.DTO.PatternResult;
using MapsterMapper;
using MantenimientoFlota.Application.Mapper;
using MantenimientoFlota.Domain.DTO.Proveedor;
using MantenimientoFlota.Application.Interfaces;
using MantenimientoFlota.Infrastructure.Interfaces;
using MantenimientoFlota.Domain.Entities;

namespace MantenimientoFlota.Application.Implementation;

public class ProveedorService : IProveedorService
{
    private readonly IProveedorRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<ProveedorService> _logger;

    public ProveedorService(IProveedorRepository repository, IMapper mapper, ILogger<ProveedorService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<ProveedorDto>>> GetAllAsync()
    {
        var proveedores = await _repository.GetAllAsync();

        _logger.LogDebug("Operación de negocio completada. Operation={Operation}", "Proveedor.GetAllAsync");

        return Result<IReadOnlyList<ProveedorDto>>.Success(_mapper.Map<List<ProveedorDto>>(proveedores), 200);
    }

    public async Task<Result<ProveedorDetalleDto>> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new InvalidRequestException("El identificador debe ser mayor que cero.");
        var entidad = await _repository.GetDetalleByIdAsync(id);
        if (entidad is null) throw new ResourceNotFoundException($"No se encontró el proveedor con id {id}.");
        _logger.LogDebug("Operación de negocio completada. Operation={Operation}", "Proveedor.GetByIdAsync");
        return Result<ProveedorDetalleDto>.Success(_mapper.Map<ProveedorDetalleDto>(entidad), 200);
    }

    public async Task<Result<ProveedorDto>> CreateAsync(
        CrearProveedorDto dto)
    {
        var proveedor = _mapper.Map<Proveedor>(dto);

        var proveedorCreado = await _repository.AddAsync(proveedor);

        _logger.LogInformation("Operación de negocio completada. Operation={Operation}", "Proveedor.CreateAsync");

        return Result<ProveedorDto>.Success(_mapper.Map<ProveedorDto>(proveedorCreado), 201);
    }

    public async Task<Result<object?>> UpdateAsync(
        int id,
        ActualizarProveedorDto dto)
    {
        if (id <= 0)
            throw new InvalidRequestException("El identificador debe ser mayor que cero.");
        var proveedor = await _repository.GetForUpdateAsync(id);

        if (proveedor is null)
        {
            throw new ResourceNotFoundException($"No se encontró el proveedor con id {id}.");
        }

        _mapper.Map(dto, proveedor);

        await _repository.UpdateAsync(proveedor);

        _logger.LogInformation("Operación de negocio completada. Operation={Operation}", "Proveedor.UpdateAsync");

        return Result<object?>.Success(null, 204);
    }

    public async Task<Result<object?>> DeleteAsync(int id)
    {
        if (id <= 0)
            throw new InvalidRequestException("El identificador debe ser mayor que cero.");
        var proveedor = await _repository.GetForUpdateAsync(id);

        if (proveedor is null)
        {
            throw new ResourceNotFoundException($"No se encontró el proveedor con id {id}.");
        }

        if (await _repository.HasAssociatedPartsAsync(id))
        {
            throw new ConflictException("No se puede eliminar el proveedor porque tiene repuestos asociados.");
        }

        await _repository.DeleteAsync(proveedor);

        _logger.LogInformation("Operación de negocio completada. Operation={Operation}", "Proveedor.DeleteAsync");

        return Result<object?>.Success(null, 204);
    }

}
