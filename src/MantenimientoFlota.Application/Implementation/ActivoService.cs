using MantenimientoFlota.Application.Common.Results;
using MantenimientoFlota.Application.DTOs.Activo;
using MantenimientoFlota.Application.Interfaces.Services;
using MantenimientoFlota.Application.Interfaces.Repositories;
using MantenimientoFlota.Domain.Entities;
using MantenimientoFlota.Domain.Enums;

namespace MantenimientoFlota.Application.Implementation;

public class ActivoService : IActivoService
{
    private readonly IActivoRepository _repository;

    public ActivoService(IActivoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ActivoDto>> GetAllAsync(
        int? categoriaActivoId = null,
        EstadoActivo? estado = null)
    {
        var activos = await _repository.GetAllAsync(categoriaActivoId, estado);

        return activos
            .Select(MapToDto)
            .ToList();
    }

    public async Task<ActivoDetalleDto?> GetByIdAsync(int id)
    {
        var fechaReferencia = DateOnly.FromDateTime(DateTime.UtcNow);
        var activo = await _repository.GetDetalleByIdAsync(id);

        if (activo is null)
        {
            return null;
        }

        activo.AntiguedadAnios = CalcularAntiguedadAnios(activo.FechaAdquisicion, fechaReferencia);
        return activo;
    }

    private static int CalcularAntiguedadAnios(DateOnly fechaAdquisicion, DateOnly fechaReferencia)
    {
        if (fechaAdquisicion > fechaReferencia)
        {
            return 0;
        }

        var anios = fechaReferencia.Year - fechaAdquisicion.Year;
        if (fechaAdquisicion.AddYears(anios) > fechaReferencia)
        {
            anios--;
        }

        return anios;
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

        if (activoExistente.Estado != dto.Estado!.Value
            && await TieneConflictoEstadoAdministrativoAsync(id, dto.Estado.Value))
        {
            return new(ActivoOperationStatus.HasOpenMaintenanceOrders);
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

    public async Task<ActivoOperationResult> CambiarEstadoAdministrativoAsync(
        int id,
        CambiarEstadoActivoDto dto)
    {
        if (dto.Estado is not (EstadoActivo.FueraDeServicio or EstadoActivo.DadoDeBaja))
        {
            return new(ActivoOperationStatus.InvalidAdministrativeState);
        }

        var activo = await _repository.GetByIdAsync(id);
        if (activo is null)
        {
            return new(ActivoOperationStatus.DoesNotExist);
        }

        if (await TieneConflictoEstadoAdministrativoAsync(id, dto.Estado.Value))
        {
            return new(ActivoOperationStatus.HasOpenMaintenanceOrders);
        }

        if (activo.Estado == dto.Estado.Value)
        {
            return new(ActivoOperationStatus.Succeeded);
        }

        activo.Estado = dto.Estado.Value;
        _repository.UpdateEstado(activo);
        return new(ActivoOperationStatus.Succeeded);
    }

    private async Task<bool> TieneConflictoEstadoAdministrativoAsync(int id, EstadoActivo destino)
    {
        return destino is EstadoActivo.FueraDeServicio or EstadoActivo.DadoDeBaja
            && await _repository.HasOpenMaintenanceOrdersAsync(id);
    }

    public async Task<ActivoOperationResult> DeleteAsync(int id)
    {
        var activo = await _repository.GetByIdAsync(id);

        if (activo is null)
        {
            return new(ActivoOperationStatus.DoesNotExist);
        }

        if (await _repository.HasAssociatedMaintenanceOrdersAsync(id))
        {
            return new(ActivoOperationStatus.HasAssociatedMaintenanceOrders);
        }

        if (activo.Estado == EstadoActivo.DadoDeBaja)
        {
            return new(ActivoOperationStatus.Succeeded);
        }

        activo.Estado = EstadoActivo.DadoDeBaja;
        _repository.UpdateEstado(activo);

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
