using MantenimientoFlota.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using MantenimientoFlota.Domain.DTO.PatternResult;
using MapsterMapper;
using MantenimientoFlota.Application.Mapper;
using MantenimientoFlota.Domain.DTO.CategoriaActivo;
using MantenimientoFlota.Application.Interfaces;
using MantenimientoFlota.Infrastructure.Interfaces;
using MantenimientoFlota.Domain.Entities;

namespace MantenimientoFlota.Application.Implementation;

public class CategoriaActivoService : ICategoriaActivoService
{
    private readonly ICategoriaActivoRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<CategoriaActivoService> _logger;

    public CategoriaActivoService(ICategoriaActivoRepository repository, IMapper mapper, ILogger<CategoriaActivoService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<CategoriaActivoDto>>> GetAllAsync()
    {
        var categorias = await _repository.GetAllAsync();

        _logger.LogDebug("Operación de negocio completada. Operation={Operation}", "CategoriaActivo.GetAllAsync");

        return Result<IReadOnlyList<CategoriaActivoDto>>.Success(_mapper.Map<List<CategoriaActivoDto>>(categorias), 200);
    }

    public async Task<Result<CategoriaActivoDetalleDto>> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new InvalidRequestException("El identificador debe ser mayor que cero.");
        var entidad = await _repository.GetDetalleByIdAsync(id);
        if (entidad is null) throw new ResourceNotFoundException($"No se encontró la categoría de activo con id {id}.");
        _logger.LogDebug("Operación de negocio completada. Operation={Operation}", "CategoriaActivo.GetByIdAsync");
        return Result<CategoriaActivoDetalleDto>.Success(entidad, 200);
    }

    public async Task<Result<CategoriaActivoDto>> CreateAsync(
        CrearCategoriaActivoDto dto)
    {
        var nombre = dto.Nombre.Trim();

        if (await _repository.ExistsByNameAsync(nombre))
        {
            throw new ConflictException("Ya existe una categoría de activo con el nombre indicado.");
        }

        var categoria = _mapper.Map<CategoriaActivo>(dto);

        var categoriaCreada = await _repository.AddAsync(categoria)
            ?? categoria;

        _logger.LogInformation("Operación de negocio completada. Operation={Operation}", "CategoriaActivo.CreateAsync");

        return Result<CategoriaActivoDto>.Success(_mapper.Map<CategoriaActivoDto>(categoriaCreada), 201);
    }

    public async Task<Result<object?>> UpdateAsync(
        int id,
        ActualizarCategoriaActivoDto dto)
    {
        if (id <= 0)
            throw new InvalidRequestException("El identificador debe ser mayor que cero.");
        var categoria = await _repository.GetForUpdateAsync(id);

        if (categoria is null)
        {
            throw new ResourceNotFoundException($"No se encontró la categoría de activo con id {id}.");
        }

        var nombre = dto.Nombre.Trim();

        if (await _repository.ExistsByNameAsync(nombre, id))
        {
            throw new ConflictException("Ya existe una categoría de activo con el nombre indicado.");
        }

        _mapper.Map(dto, categoria);

        await _repository.UpdateAsync(categoria);

        _logger.LogInformation("Operación de negocio completada. Operation={Operation}", "CategoriaActivo.UpdateAsync");

        return Result<object?>.Success(null, 204);
    }

    public async Task<Result<object?>> DeleteAsync(int id)
    {
        if (id <= 0)
            throw new InvalidRequestException("El identificador debe ser mayor que cero.");
        var categoria = await _repository.GetForUpdateAsync(id);

        if (categoria is null)
        {
            throw new ResourceNotFoundException($"No se encontró la categoría de activo con id {id}.");
        }

        if (await _repository.HasAssociatedAssetsAsync(id))
        {
            throw new ConflictException("No se puede eliminar la categoría porque tiene activos asociados.");
        }

        await _repository.DeleteAsync(id);

        _logger.LogInformation("Operación de negocio completada. Operation={Operation}", "CategoriaActivo.DeleteAsync");

        return Result<object?>.Success(null, 204);
    }

}
