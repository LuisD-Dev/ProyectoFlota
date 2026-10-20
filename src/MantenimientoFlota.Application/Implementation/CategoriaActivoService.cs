using MantenimientoFlota.Application.Common.Exceptions;
using MantenimientoFlota.Application.DTOs.CategoriaActivo;
using MantenimientoFlota.Application.Interfaces.Services;
using MantenimientoFlota.Application.Interfaces.Repositories;
using MantenimientoFlota.Domain.Entities;

namespace MantenimientoFlota.Application.Implementation;

public class CategoriaActivoService : ICategoriaActivoService
{
    private readonly ICategoriaActivoRepository _repository;

    public CategoriaActivoService(ICategoriaActivoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<CategoriaActivoDto>> GetAllAsync()
    {
        var categorias = await _repository.GetAllAsync();

        return categorias
            .Select(MapToDto)
            .ToList();
    }

    public async Task<CategoriaActivoDetalleDto> GetByIdAsync(int id)
    {
        ValidarId(id);
        return await _repository.GetDetalleByIdAsync(id)
            ?? throw new ResourceNotFoundException($"No se encontró la categoría de activo con id {id}.");
    }

    public async Task<CategoriaActivoDto> CreateAsync(
        CrearCategoriaActivoDto dto)
    {
        var nombre = dto.Nombre.Trim();

        if (await _repository.ExistsByNameAsync(nombre))
        {
            throw new ConflictException("Ya existe una categoría de activo con el nombre indicado.");
        }

        var categoria = new CategoriaActivo
        {
            Nombre = nombre,
            Descripcion = dto.Descripcion ?? string.Empty,
            IntervaloMantenimientoKm = dto.IntervaloMantenimientoKm
        };

        var categoriaCreada = await _repository.AddAsync(categoria)
            ?? categoria;

        return MapToDto(categoriaCreada);
    }

    public async Task UpdateAsync(
        int id,
        ActualizarCategoriaActivoDto dto)
    {
        ValidarId(id);
        var categoria = await _repository.GetByIdAsync(id);

        if (categoria is null)
        {
            throw new ResourceNotFoundException($"No se encontró la categoría de activo con id {id}.");
        }

        var nombre = dto.Nombre.Trim();

        if (await _repository.ExistsByNameAsync(nombre, id))
        {
            throw new ConflictException("Ya existe una categoría de activo con el nombre indicado.");
        }

        categoria.Nombre = nombre;
        categoria.Descripcion = dto.Descripcion ?? string.Empty;
        categoria.IntervaloMantenimientoKm = dto.IntervaloMantenimientoKm;

        _repository.Update(categoria);
    }

    public async Task DeleteAsync(int id)
    {
        ValidarId(id);
        var categoria = await _repository.GetByIdAsync(id);

        if (categoria is null)
        {
            throw new ResourceNotFoundException($"No se encontró la categoría de activo con id {id}.");
        }

        if (await _repository.HasAssociatedAssetsAsync(id))
        {
            throw new ConflictException("No se puede eliminar la categoría porque tiene activos asociados.");
        }

        _repository.Delete(id);
    }

    private static void ValidarId(int id)
    {
        if (id <= 0)
            throw new InvalidRequestException("El identificador debe ser mayor que cero.");
    }

    private static CategoriaActivoDto MapToDto(CategoriaActivo categoria)
    {
        return new CategoriaActivoDto
        {
            Id = categoria.Id,
            Nombre = categoria.Nombre,
            Descripcion = categoria.Descripcion,
            IntervaloMantenimientoKm = categoria.IntervaloMantenimientoKm
        };
    }
}
