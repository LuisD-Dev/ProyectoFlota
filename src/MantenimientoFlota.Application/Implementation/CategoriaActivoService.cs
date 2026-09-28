using MantenimientoFlota.Application.Common.Results;
using MantenimientoFlota.Application.DTOs.CategoriaActivo;
using MantenimientoFlota.Application.Interfaces;
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

    public async Task<CategoriaActivoDto?> GetByIdAsync(int id)
    {
        var categoria = await _repository.GetByIdAsync(id);

        return categoria is null
            ? null
            : MapToDto(categoria);
    }

    public async Task<CategoriaActivoOperationResult> CreateAsync(
        CrearCategoriaActivoDto dto)
    {
        var nombre = dto.Nombre.Trim();

        if (await _repository.ExistsByNameAsync(nombre))
        {
            return new(CategoriaActivoOperationStatus.NameAlreadyExists);
        }

        var categoria = new CategoriaActivo
        {
            Nombre = nombre,
            Descripcion = dto.Descripcion ?? string.Empty,
            IntervaloMantenimientoKm = dto.IntervaloMantenimientoKm
        };

        var categoriaCreada = await _repository.AddAsync(categoria)
            ?? categoria;

        return new(
            CategoriaActivoOperationStatus.Succeeded,
            MapToDto(categoriaCreada));
    }

    public async Task<CategoriaActivoOperationResult> UpdateAsync(
        int id,
        ActualizarCategoriaActivoDto dto)
    {
        var categoria = await _repository.GetByIdAsync(id);

        if (categoria is null)
        {
            return new(CategoriaActivoOperationStatus.DoesNotExist);
        }

        var nombre = dto.Nombre.Trim();

        if (await _repository.ExistsByNameAsync(nombre, id))
        {
            return new(CategoriaActivoOperationStatus.NameAlreadyExists);
        }

        categoria.Nombre = nombre;
        categoria.Descripcion = dto.Descripcion ?? string.Empty;
        categoria.IntervaloMantenimientoKm = dto.IntervaloMantenimientoKm;

        _repository.Update(categoria);

        return new(CategoriaActivoOperationStatus.Succeeded);
    }

    public async Task<CategoriaActivoOperationResult> DeleteAsync(int id)
    {
        var categoria = await _repository.GetByIdAsync(id);

        if (categoria is null)
        {
            return new(CategoriaActivoOperationStatus.DoesNotExist);
        }

        if (await _repository.HasAssociatedAssetsAsync(id))
        {
            return new(CategoriaActivoOperationStatus.HasAssociatedAssets);
        }

        _repository.Delete(id);

        return new(CategoriaActivoOperationStatus.Succeeded);
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
