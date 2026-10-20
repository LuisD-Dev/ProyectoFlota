using MantenimientoFlota.Application.Common.Exceptions;
using MantenimientoFlota.Application.DTOs.Proveedor;
using MantenimientoFlota.Application.Interfaces.Services;
using MantenimientoFlota.Application.Interfaces.Repositories;
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

    public async Task<ProveedorDetalleDto> GetByIdAsync(int id)
    {
        ValidarId(id);
        return await _repository.GetDetalleByIdAsync(id)
            ?? throw new ResourceNotFoundException($"No se encontró el proveedor con id {id}.");
    }

    public async Task<ProveedorDto> CreateAsync(
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

        return MapToDto(proveedorCreado);
    }

    public async Task UpdateAsync(
        int id,
        ActualizarProveedorDto dto)
    {
        ValidarId(id);
        var proveedor = await _repository.GetByIdAsync(id);

        if (proveedor is null)
        {
            throw new ResourceNotFoundException($"No se encontró el proveedor con id {id}.");
        }

        proveedor.Nombre = dto.Nombre.Trim();
        proveedor.Contacto = dto.Contacto?.Trim() ?? string.Empty;
        proveedor.Direccion = dto.Direccion?.Trim() ?? string.Empty;
        proveedor.CondicionesGenerales = dto.CondicionesGenerales?.Trim() ?? string.Empty;

        await _repository.UpdateAsync(proveedor);
    }

    public async Task DeleteAsync(int id)
    {
        ValidarId(id);
        var proveedor = await _repository.GetByIdAsync(id);

        if (proveedor is null)
        {
            throw new ResourceNotFoundException($"No se encontró el proveedor con id {id}.");
        }

        if (await _repository.HasAssociatedPartsAsync(id))
        {
            throw new ConflictException("No se puede eliminar el proveedor porque tiene repuestos asociados.");
        }

        await _repository.DeleteAsync(proveedor);
    }

    private static void ValidarId(int id)
    {
        if (id <= 0)
            throw new InvalidRequestException("El identificador debe ser mayor que cero.");
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
