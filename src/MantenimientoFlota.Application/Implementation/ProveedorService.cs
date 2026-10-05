using MantenimientoFlota.Application.Common.Results;
using MantenimientoFlota.Application.DTOs.Proveedor;
using MantenimientoFlota.Application.Interfaces;
using MantenimientoFlota.Domain.Entities;

namespace MantenimientoFlota.Application.Implementation;

public class ProveedorService : IProveedorService
{
    private readonly IProveedorRepository _repository;

    public ProveedorService(IProveedorRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ProveedorDto>> GetAllAsync()
    {
        var proveedores = await _repository.GetAllAsync();

        return proveedores
            .Select(MapToDto)
            .ToList();
    }

    public async Task<ProveedorDto?> GetByIdAsync(int id)
    {
        var proveedor = await _repository.GetByIdAsync(id);

        return proveedor is null
            ? null
            : MapToDto(proveedor);
    }

    public async Task<ProveedorOperationResult> CreateAsync(
        CrearProveedorDto dto)
    {
        var proveedor = new Proveedor
        {
            Nombre = dto.Nombre.Trim(),
            Contacto = dto.Contacto?.Trim() ?? string.Empty,
            Direccion = dto.Direccion?.Trim() ?? string.Empty,
            CondicionesGenerales = dto.CondicionesGenerales?.Trim() ?? string.Empty
        };

        var proveedorCreado = await _repository.AddAsync(proveedor);

        return new(
            ProveedorOperationStatus.Succeeded,
            MapToDto(proveedorCreado));
    }

    public async Task<ProveedorOperationResult> UpdateAsync(
        int id,
        ActualizarProveedorDto dto)
    {
        var proveedor = await _repository.GetByIdAsync(id);

        if (proveedor is null)
        {
            return new(ProveedorOperationStatus.DoesNotExist);
        }

        proveedor.Nombre = dto.Nombre.Trim();
        proveedor.Contacto = dto.Contacto?.Trim() ?? string.Empty;
        proveedor.Direccion = dto.Direccion?.Trim() ?? string.Empty;
        proveedor.CondicionesGenerales = dto.CondicionesGenerales?.Trim() ?? string.Empty;

        await _repository.UpdateAsync(proveedor);

        return new(ProveedorOperationStatus.Succeeded);
    }

    public async Task<ProveedorOperationResult> DeleteAsync(int id)
    {
        var proveedor = await _repository.GetByIdAsync(id);

        if (proveedor is null)
        {
            return new(ProveedorOperationStatus.DoesNotExist);
        }

        if (await _repository.HasAssociatedPartsAsync(id))
        {
            return new(ProveedorOperationStatus.HasAssociatedParts);
        }

        await _repository.DeleteAsync(proveedor);

        return new(ProveedorOperationStatus.Succeeded);
    }

    private static ProveedorDto MapToDto(Proveedor proveedor)
    {
        return new ProveedorDto
        {
            Id = proveedor.Id,
            Nombre = proveedor.Nombre,
            Contacto = proveedor.Contacto,
            Direccion = proveedor.Direccion,
            CondicionesGenerales = proveedor.CondicionesGenerales
        };
    }
}
