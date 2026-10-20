using MantenimientoFlota.Application.Common.Exceptions;
using MantenimientoFlota.Application.DTOs.RepuestoProveedor;
using MantenimientoFlota.Application.Interfaces.Services;
using MantenimientoFlota.Application.Interfaces.Repositories;
using MantenimientoFlota.Domain.Entities;

namespace MantenimientoFlota.Application.Implementation;

public class RepuestoProveedorService : IRepuestoProveedorService
{
    private readonly IRepuestoProveedorRepository _repository;

    public RepuestoProveedorService(
        IRepuestoProveedorRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<RepuestoProveedorDto>> GetAllAsync()
    {
        var repuestoProveedores = await _repository.GetAllAsync();

        return repuestoProveedores
            .Select(MapToDto)
            .ToList();
    }

    public async Task<RepuestoProveedorDto> GetByIdAsync(
        int repuestoId,
        int proveedorId)
    {
        ValidarIds(repuestoId, proveedorId);
        var repuestoProveedor = await _repository.GetByIdAsync(
            repuestoId,
            proveedorId);

        return repuestoProveedor is null
            ? throw new ResourceNotFoundException($"No se encontró la relación entre el repuesto con id {repuestoId} y el proveedor con id {proveedorId}.")
            : MapToDto(repuestoProveedor);
    }

    public async Task<RepuestoProveedorDto> CreateAsync(
        CrearRepuestoProveedorDto dto)
    {
        var repuestoId = dto.RepuestoId!.Value;
        var proveedorId = dto.ProveedorId!.Value;

        if (!await _repository.RepuestoExistsAsync(repuestoId))
        {
            throw new ResourceNotFoundException($"No se encontró el repuesto con id {dto.RepuestoId}.");
        }

        if (!await _repository.ProveedorExistsAsync(proveedorId))
        {
            throw new ResourceNotFoundException($"No se encontró el proveedor con id {dto.ProveedorId}.");
        }

        if (await _repository.ExistsAsync(repuestoId, proveedorId))
        {
            throw new ConflictException("Ya existe una relación entre el repuesto y el proveedor indicados.");
        }

        var repuestoProveedor = new RepuestoProveedor
        {
            RepuestoId = repuestoId,
            ProveedorId = proveedorId,
            PrecioOfrecido = dto.PrecioOfrecido!.Value,
            TiempoEntregaDias = dto.TiempoEntregaDias!.Value
        };

        var relacionCreada = await _repository.AddAsync(repuestoProveedor);
        var relacionCompleta = await _repository.GetByIdAsync(
            relacionCreada.RepuestoId,
            relacionCreada.ProveedorId)
            ?? relacionCreada;

        return MapToDto(relacionCompleta);
    }

    public async Task UpdateAsync(
        int repuestoId,
        int proveedorId,
        ActualizarRepuestoProveedorDto dto)
    {
        ValidarIds(repuestoId, proveedorId);
        if (!await _repository.ExistsAsync(repuestoId, proveedorId))
        {
            throw new ResourceNotFoundException($"No se encontró la relación entre el repuesto con id {repuestoId} y el proveedor con id {proveedorId}.");
        }

        var repuestoProveedor = new RepuestoProveedor
        {
            RepuestoId = repuestoId,
            ProveedorId = proveedorId,
            PrecioOfrecido = dto.PrecioOfrecido!.Value,
            TiempoEntregaDias = dto.TiempoEntregaDias!.Value
        };

        await _repository.UpdateAsync(repuestoProveedor);

    }

    public async Task DeleteAsync(
        int repuestoId,
        int proveedorId)
    {
        ValidarIds(repuestoId, proveedorId);
        if (!await _repository.ExistsAsync(repuestoId, proveedorId))
        {
            throw new ResourceNotFoundException($"No se encontró la relación entre el repuesto con id {repuestoId} y el proveedor con id {proveedorId}.");
        }

        await _repository.DeleteAsync(repuestoId, proveedorId);

    }

    private static void ValidarIds(int repuestoId, int proveedorId)
    {
        if (repuestoId <= 0 || proveedorId <= 0)
            throw new InvalidRequestException("Los identificadores de repuesto y proveedor deben ser mayores que cero.");
    }

    private static RepuestoProveedorDto MapToDto(
        RepuestoProveedor repuestoProveedor)
    {
        return new RepuestoProveedorDto
        {
            RepuestoId = repuestoProveedor.RepuestoId,
            RepuestoNombre = repuestoProveedor.Repuesto?.Nombre
                ?? string.Empty,
            ProveedorId = repuestoProveedor.ProveedorId,
            ProveedorNombre = repuestoProveedor.Proveedor?.Nombre
                ?? string.Empty,
            PrecioOfrecido = repuestoProveedor.PrecioOfrecido,
            TiempoEntregaDias = repuestoProveedor.TiempoEntregaDias
        };
    }
}
