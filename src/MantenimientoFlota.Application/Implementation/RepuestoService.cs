using MantenimientoFlota.Application.Common.Exceptions;
using MantenimientoFlota.Application.DTOs.Repuesto;
using MantenimientoFlota.Application.Interfaces.Services;
using MantenimientoFlota.Application.Interfaces.Repositories;
using MantenimientoFlota.Domain.Entities;

namespace MantenimientoFlota.Application.Implementation;

public class RepuestoService : IRepuestoService
{
    private readonly IRepuestoRepository _repository;

    public RepuestoService(IRepuestoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<RepuestoDto>> GetAllAsync(bool stockBajo = false)
    {
        var repuestos = await _repository.GetAllAsync(stockBajo);

        return repuestos
            .Select(MapToDto)
            .ToList();
    }

    public async Task<RepuestoDetalleDto> GetByIdAsync(int id)
    {
        ValidarId(id);
        return await _repository.GetDetalleByIdAsync(id)
            ?? throw new ResourceNotFoundException($"No se encontró el repuesto con id {id}.");
    }

    public async Task<RepuestoDto> CreateAsync(
        CrearRepuestoDto dto)
    {
        var codigo = dto.Codigo.Trim();

        if (await _repository.ExistsByCodigoAsync(codigo))
        {
            throw new ConflictException("Ya existe un repuesto con el código indicado.");
        }

        var repuesto = new Repuesto
        {
            Codigo = codigo,
            Nombre = dto.Nombre.Trim(),
            UnidadMedida = dto.UnidadMedida.Trim(),
            StockActual = dto.StockActual,
            StockMinimo = dto.StockMinimo
        };

        var repuestoCreado = await _repository.AddAsync(repuesto) ?? repuesto;

        return MapToDto(repuestoCreado);
    }

    public async Task UpdateAsync(
        int id,
        ActualizarRepuestoDto dto)
    {
        ValidarId(id);
        var repuesto = await _repository.GetByIdAsync(id);

        if (repuesto is null)
        {
            throw new ResourceNotFoundException($"No se encontró el repuesto con id {id}.");
        }

        var codigo = dto.Codigo.Trim();

        if (await _repository.ExistsByCodigoAsync(codigo, id))
        {
            throw new ConflictException("Ya existe un repuesto con el código indicado.");
        }

        repuesto.Codigo = codigo;
        repuesto.Nombre = dto.Nombre.Trim();
        repuesto.UnidadMedida = dto.UnidadMedida.Trim();
        repuesto.StockActual = dto.StockActual;
        repuesto.StockMinimo = dto.StockMinimo;

        _repository.Update(repuesto);
    }

    public async Task DeleteAsync(int id)
    {
        ValidarId(id);
        var repuesto = await _repository.GetByIdAsync(id);

        if (repuesto is null)
        {
            throw new ResourceNotFoundException($"No se encontró el repuesto con id {id}.");
        }

        if (await _repository.HasAssociatedSuppliersAsync(id))
        {
            throw new ConflictException("No se puede eliminar el repuesto porque tiene proveedores asociados.");
        }

        _repository.Delete(id);
    }

    private static void ValidarId(int id)
    {
        if (id <= 0)
            throw new InvalidRequestException("El identificador debe ser mayor que cero.");
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
