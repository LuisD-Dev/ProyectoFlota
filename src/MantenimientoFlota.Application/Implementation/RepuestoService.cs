using MantenimientoFlota.Application.Common.Results;
using MantenimientoFlota.Application.DTOs.Repuesto;
using MantenimientoFlota.Application.Interfaces;
using MantenimientoFlota.Domain.Entities;

namespace MantenimientoFlota.Application.Implementation;

public class RepuestoService : IRepuestoService
{
    private readonly IRepuestoRepository _repository;

    public RepuestoService(IRepuestoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<RepuestoDto>> GetAllAsync()
    {
        var repuestos = await _repository.GetAllAsync();

        return repuestos
            .Select(MapToDto)
            .ToList();
    }

    public async Task<RepuestoDto?> GetByIdAsync(int id)
    {
        var repuesto = await _repository.GetByIdAsync(id);

        return repuesto is null
            ? null
            : MapToDto(repuesto);
    }

    public async Task<RepuestoOperationResult> CreateAsync(
        CrearRepuestoDto dto)
    {
        var repuesto = new Repuesto
        {
            Codigo = dto.Codigo.Trim(),
            Nombre = dto.Nombre.Trim(),
            UnidadMedida = dto.UnidadMedida.Trim(),
            StockActual = dto.StockActual,
            StockMinimo = dto.StockMinimo
        };

        var repuestoCreado = await _repository.AddAsync(repuesto) ?? repuesto;

        return new(
            RepuestoOperationStatus.Succeeded,
            MapToDto(repuestoCreado));
    }

    public async Task<RepuestoOperationResult> UpdateAsync(
        int id,
        ActualizarRepuestoDto dto)
    {
        var repuesto = await _repository.GetByIdAsync(id);

        if (repuesto is null)
        {
            return new(RepuestoOperationStatus.DoesNotExist);
        }

        repuesto.Codigo = dto.Codigo.Trim();
        repuesto.Nombre = dto.Nombre.Trim();
        repuesto.UnidadMedida = dto.UnidadMedida.Trim();
        repuesto.StockActual = dto.StockActual;
        repuesto.StockMinimo = dto.StockMinimo;

        _repository.Update(repuesto);

        return new(RepuestoOperationStatus.Succeeded);
    }

    public async Task<RepuestoOperationResult> DeleteAsync(int id)
    {
        var repuesto = await _repository.GetByIdAsync(id);

        if (repuesto is null)
        {
            return new(RepuestoOperationStatus.DoesNotExist);
        }

        if (await _repository.HasAssociatedSuppliersAsync(id))
        {
            return new(RepuestoOperationStatus.HasAssociatedSuppliers);
        }

        _repository.Delete(id);

        return new(RepuestoOperationStatus.Succeeded);
    }

    private static RepuestoDto MapToDto(Repuesto repuesto)
    {
        return new RepuestoDto
        {
            Id = repuesto.Id,
            Codigo = repuesto.Codigo,
            Nombre = repuesto.Nombre,
            UnidadMedida = repuesto.UnidadMedida,
            StockActual = repuesto.StockActual,
            StockMinimo = repuesto.StockMinimo
        };
    }
}
