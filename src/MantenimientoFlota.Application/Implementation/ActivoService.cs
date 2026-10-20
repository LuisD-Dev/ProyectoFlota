using MantenimientoFlota.Application.Common.Results;
using MantenimientoFlota.Application.Common.Exceptions;
using MantenimientoFlota.Application.DTOs.Activo;
using MantenimientoFlota.Application.Interfaces.Services;
using MantenimientoFlota.Application.Interfaces.Repositories;
using MantenimientoFlota.Domain.Entities;
using MantenimientoFlota.Domain.Enums;

namespace MantenimientoFlota.Application.Implementation;

public class ActivoService : IActivoService
{
    private readonly IActivoRepository _repository;
    private readonly IActivoImagenStorage _imagenes;

    public ActivoService(IActivoRepository repository, IActivoImagenStorage imagenes)
    {
        _repository = repository;
        _imagenes = imagenes;
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

    public async Task<ActivoDetalleDto> GetByIdAsync(int id)
    {
        ValidarId(id);
        var fechaReferencia = DateOnly.FromDateTime(DateTime.UtcNow);
        var activo = await _repository.GetDetalleByIdAsync(id);

        if (activo is null)
        {
            throw new ResourceNotFoundException($"No se encontró el activo con id {id}.");
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

    public async Task<ActivoDto> CreateAsync(CrearActivoDto dto)
    {
        if (_imagenes.EsRutaReservada(dto.ImagenUrl))
        {
            throw new InvalidRequestException("Las rutas de imágenes administradas solo pueden asignarse mediante la carga de imagen del activo.");
        }

        var codigo = dto.Codigo.Trim();

        if (await _repository.ExistsByCodigoAsync(codigo))
        {
            throw new ConflictException("Ya existe un activo con el código indicado.");
        }

        var categoriaActivoId = dto.CategoriaActivoId!.Value;

        if (!await _repository.CategoriaExisteAsync(categoriaActivoId))
        {
            throw new InvalidRequestException("No existe una categoría de activo con el identificador indicado.");
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

        return MapToDto(activoCompleto);
    }

    public async Task UpdateAsync(
        int id,
        ActualizarActivoDto dto)
    {
        ValidarId(id);
        var activoExistente = await _repository.GetByIdAsync(id);

        if (activoExistente is null)
        {
            throw new ResourceNotFoundException($"No se encontró el activo con id {id}.");
        }

        if (_imagenes.EsRutaReservada(dto.ImagenUrl)
            && !(dto.ImagenUrl == activoExistente.ImagenUrl
                && _imagenes.EsRutaAdministrada(id, activoExistente.ImagenUrl)))
        {
            throw new InvalidRequestException("Las rutas de imágenes administradas solo pueden asignarse mediante la carga de imagen del activo.");
        }

        var codigo = dto.Codigo.Trim();

        if (await _repository.ExistsByCodigoAsync(codigo, id))
        {
            throw new ConflictException("Ya existe un activo con el código indicado.");
        }

        var categoriaActivoId = dto.CategoriaActivoId!.Value;

        if (!await _repository.CategoriaExisteAsync(categoriaActivoId))
        {
            throw new InvalidRequestException("No existe una categoría de activo con el identificador indicado.");
        }

        if (activoExistente.Estado != dto.Estado!.Value
            && await TieneConflictoEstadoAdministrativoAsync(id, dto.Estado.Value))
        {
            throw new ConflictException("No se puede cambiar el estado del activo porque tiene órdenes de mantenimiento abiertas.");
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
            ImagenUrl = _imagenes.EsRutaAdministrada(id, activoExistente.ImagenUrl)
                ? activoExistente.ImagenUrl : dto.ImagenUrl ?? string.Empty,
            Placa = dto.Placa ?? string.Empty,
            KilometrajeActual = dto.KilometrajeActual!.Value,
            CategoriaActivoId = categoriaActivoId
        };

        _repository.Update(activoActualizado);

        return;
    }

    public async Task CambiarEstadoAdministrativoAsync(
        int id,
        CambiarEstadoActivoDto dto)
    {
        ValidarId(id);
        if (dto.Estado is not (EstadoActivo.FueraDeServicio or EstadoActivo.DadoDeBaja))
        {
            throw new InvalidRequestException("Solo se permite establecer el estado FueraDeServicio o DadoDeBaja.");
        }

        var activo = await _repository.GetByIdAsync(id);
        if (activo is null)
        {
            throw new ResourceNotFoundException($"No se encontró el activo con id {id}.");
        }

        if (await TieneConflictoEstadoAdministrativoAsync(id, dto.Estado.Value))
        {
            throw new ConflictException("No se puede cambiar el estado del activo porque tiene órdenes de mantenimiento abiertas.");
        }

        if (activo.Estado == dto.Estado.Value)
        {
            return;
        }

        activo.Estado = dto.Estado.Value;
        _repository.UpdateEstado(activo);
        return;
    }

    private async Task<bool> TieneConflictoEstadoAdministrativoAsync(int id, EstadoActivo destino)
    {
        return destino is EstadoActivo.FueraDeServicio or EstadoActivo.DadoDeBaja
            && await _repository.HasOpenMaintenanceOrdersAsync(id);
    }

    public async Task DeleteAsync(int id)
    {
        ValidarId(id);
        var activo = await _repository.GetByIdAsync(id);

        if (activo is null)
        {
            throw new ResourceNotFoundException($"No se encontró el activo con id {id}.");
        }

        if (await _repository.HasAssociatedMaintenanceOrdersAsync(id))
        {
            throw new ConflictException("No se puede eliminar el activo porque tiene órdenes de mantenimiento asociadas.");
        }

        if (activo.Estado == EstadoActivo.DadoDeBaja)
        {
            return;
        }

        activo.Estado = EstadoActivo.DadoDeBaja;
        _repository.UpdateEstado(activo);

        return;
    }

    public async Task<ActivoImagenDto> ActualizarImagenAsync(
        int id, Stream contenido, string extension, CancellationToken cancellationToken)
    {
        ValidarId(id);
        var activo = await _repository.GetByIdAsync(id);
        if (activo is null) throw new ResourceNotFoundException($"No se encontró el activo con id {id}.");

        var nuevaRuta = await _imagenes.GuardarAsync(id, contenido, extension, cancellationToken);
        try
        {
            await _repository.ActualizarImagenUrlAsync(id, nuevaRuta, cancellationToken);
        }
        catch (ImagenUpdateRejectedException)
        {
            await _imagenes.EliminarSiAdministradaAsync(id, nuevaRuta);
            throw;
        }
        // Otros fallos pueden tener resultado SQL incierto: conservar ambos archivos.
        await _imagenes.EliminarSiAdministradaAsync(id, activo.ImagenUrl);
        return new(id, nuevaRuta);
    }

    private static void ValidarId(int id)
    {
        if (id <= 0)
            throw new InvalidRequestException("El identificador debe ser mayor que cero.");
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
