using MantenimientoFlota.Application.Common.Results;
using MantenimientoFlota.Application.DTOs.Activo;
using MantenimientoFlota.Application.Interfaces;
using MantenimientoFlota.Domain.Entities;

namespace MantenimientoFlota.Application.Implementation;

public class ActivoService : IActivoService
{
    private readonly IActivoRepository _repository;

    public ActivoService(IActivoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ActivoDto>> GetAllAsync()
    {
        var activos = await _repository.GetAllAsync();

        return activos
            .Select(MapToDto)
            .ToList();
    }

    public async Task<ActivoDto?> GetByIdAsync(int id)
    {
        var activo = await _repository.GetByIdAsync(id);

        return activo is null
            ? null
            : MapToDto(activo);
    }

    public async Task<ActivoOperationResult> CreateAsync(CrearActivoDto dto)
    {
        var codigo = dto.Codigo.Trim();

        if (await _repository.ExistsByCodigoAsync(codigo))
        {
            return new(ActivoOperationStatus.CodeAlreadyExists);
        }

        var categoriaActivoId = dto.CategoriaActivoId!.Value;

        if (!await _repository.CategoriaExisteAsync(categoriaActivoId))
        {
            return new(ActivoOperationStatus.CategoryDoesNotExist);
        }

        var activo = new Activo
        {
            Codigo = codigo,
            Nombre = dto.Nombre.Trim(),
            Descripcion = dto.Descripcion ?? string.Empty,
            FechaAdquisicion = dto.FechaAdquisicion!.Value,
            ValorReferencia = dto.ValorReferencia!.Value,
            Estado = dto.Estado!.Value,
            ImagenUrl = dto.ImagenUrl ?? string.Empty,
            Placa = dto.Placa ?? string.Empty,
            KilometrajeActual = dto.KilometrajeActual!.Value,
            CategoriaActivoId = categoriaActivoId
        };

        var activoCreado = await _repository.AddAsync(activo)
            ?? activo;

        var activoCompleto = await _repository.GetByIdAsync(activoCreado.Id)
            ?? activoCreado;

        return new(
            ActivoOperationStatus.Succeeded,
            MapToDto(activoCompleto));
    }

    public async Task<ActivoOperationResult> UpdateAsync(
        int id,
        ActualizarActivoDto dto)
    {
        var activoExistente = await _repository.GetByIdAsync(id);

        if (activoExistente is null)
        {
            return new(ActivoOperationStatus.DoesNotExist);
        }

        var codigo = dto.Codigo.Trim();

        if (await _repository.ExistsByCodigoAsync(codigo, id))
        {
            return new(ActivoOperationStatus.CodeAlreadyExists);
        }

        var categoriaActivoId = dto.CategoriaActivoId!.Value;

        if (!await _repository.CategoriaExisteAsync(categoriaActivoId))
        {
            return new(ActivoOperationStatus.CategoryDoesNotExist);
        }

        var activoActualizado = new Activo
        {
            Id = id,
            Codigo = codigo,
            Nombre = dto.Nombre.Trim(),
            Descripcion = dto.Descripcion ?? string.Empty,
            FechaAdquisicion = dto.FechaAdquisicion!.Value,
            ValorReferencia = dto.ValorReferencia!.Value,
            Estado = dto.Estado!.Value,
            ImagenUrl = dto.ImagenUrl ?? string.Empty,
            Placa = dto.Placa ?? string.Empty,
            KilometrajeActual = dto.KilometrajeActual!.Value,
            CategoriaActivoId = categoriaActivoId
        };

        _repository.Update(activoActualizado);

        return new(ActivoOperationStatus.Succeeded);
    }

    public async Task<ActivoOperationResult> DeleteAsync(int id)
    {
        var activo = await _repository.GetByIdAsync(id);

        if (activo is null)
        {
            return new(ActivoOperationStatus.DoesNotExist);
        }

        _repository.Delete(id);

        return new(ActivoOperationStatus.Succeeded);
    }

    private static ActivoDto MapToDto(Activo activo)
    {
        return new ActivoDto
        {
            Id = activo.Id,
            Codigo = activo.Codigo,
            Nombre = activo.Nombre,
            Descripcion = activo.Descripcion,
            FechaAdquisicion = activo.FechaAdquisicion,
            ValorReferencia = activo.ValorReferencia,
            Estado = activo.Estado,
            ImagenUrl = activo.ImagenUrl,
            Placa = activo.Placa,
            KilometrajeActual = activo.KilometrajeActual,
            CategoriaActivoId = activo.CategoriaActivoId,
            CategoriaActivoNombre = activo.CategoriaActivo?.Nombre
                ?? string.Empty
        };
    }
}
