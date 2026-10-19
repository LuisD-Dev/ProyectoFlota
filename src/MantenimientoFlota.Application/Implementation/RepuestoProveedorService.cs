using MantenimientoFlota.Application.Common.Results;
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

    public async Task<RepuestoProveedorDto?> GetByIdAsync(
        int repuestoId,
        int proveedorId)
    {
        var repuestoProveedor = await _repository.GetByIdAsync(
            repuestoId,
            proveedorId);

        return repuestoProveedor is null
            ? null
            : MapToDto(repuestoProveedor);
    }

    public async Task<RepuestoProveedorOperationResult> CreateAsync(
        CrearRepuestoProveedorDto dto)
    {
        var repuestoId = dto.RepuestoId!.Value;
        var proveedorId = dto.ProveedorId!.Value;

        if (!await _repository.RepuestoExistsAsync(repuestoId))
        {
            return new(
                RepuestoProveedorOperationStatus.RepuestoDoesNotExist);
        }

        if (!await _repository.ProveedorExistsAsync(proveedorId))
        {
            return new(
                RepuestoProveedorOperationStatus.ProveedorDoesNotExist);
        }

        if (await _repository.ExistsAsync(repuestoId, proveedorId))
        {
            return new(RepuestoProveedorOperationStatus.AlreadyExists);
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

        return new(
            RepuestoProveedorOperationStatus.Succeeded,
            MapToDto(relacionCompleta));
    }

    public async Task<RepuestoProveedorOperationResult> UpdateAsync(
        int repuestoId,
        int proveedorId,
        ActualizarRepuestoProveedorDto dto)
    {
        if (!await _repository.ExistsAsync(repuestoId, proveedorId))
        {
            return new(RepuestoProveedorOperationStatus.DoesNotExist);
        }

        var repuestoProveedor = new RepuestoProveedor
        {
            RepuestoId = repuestoId,
            ProveedorId = proveedorId,
            PrecioOfrecido = dto.PrecioOfrecido!.Value,
            TiempoEntregaDias = dto.TiempoEntregaDias!.Value
        };

        await _repository.UpdateAsync(repuestoProveedor);

        return new(RepuestoProveedorOperationStatus.Succeeded);
    }

    public async Task<RepuestoProveedorOperationResult> DeleteAsync(
        int repuestoId,
        int proveedorId)
    {
        if (!await _repository.ExistsAsync(repuestoId, proveedorId))
        {
            return new(RepuestoProveedorOperationStatus.DoesNotExist);
        }

        await _repository.DeleteAsync(repuestoId, proveedorId);

        return new(RepuestoProveedorOperationStatus.Succeeded);
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
