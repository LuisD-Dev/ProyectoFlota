using MantenimientoFlota.Application.Common.Exceptions;
using MantenimientoFlota.Application.DTOs.Tecnico;
using MantenimientoFlota.Application.Interfaces.Services;
using MantenimientoFlota.Application.Interfaces.Repositories;
using MantenimientoFlota.Domain.Entities;

namespace MantenimientoFlota.Application.Implementation;

public class TecnicoService : ITecnicoService
{
    private readonly ITecnicoRepository _repository;

    public TecnicoService(ITecnicoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TecnicoDto>> GetAllAsync(
        bool? disponible = null,
        int? especialidadId = null)
    {
        if (especialidadId is <= 0)
            throw new InvalidRequestException("El identificador de la especialidad debe ser mayor que cero.");

        var tecnicos = await _repository.GetAllAsync(disponible, especialidadId);

        return tecnicos
            .Select(MapToDto)
            .ToList();
    }

    public async Task<TecnicoDetalleDto> GetByIdAsync(int id)
    {
        ValidarId(id);
        var fechaReferencia = DateOnly.FromDateTime(DateTime.UtcNow);
        var tecnico = await _repository.GetDetalleByIdAsync(id);

        if (tecnico is null)
        {
            throw new ResourceNotFoundException($"No se encontró el técnico con id {id}.");
        }

        foreach (var especialidad in tecnico.Especialidades)
        {
            especialidad.AntiguedadAnios = CalcularAntiguedadAnios(
                especialidad.FechaObtencion, fechaReferencia);
        }

        return tecnico;
    }

    private static int CalcularAntiguedadAnios(DateOnly fechaObtencion, DateOnly fechaReferencia)
    {
        if (fechaObtencion > fechaReferencia)
        {
            return 0;
        }

        var anios = fechaReferencia.Year - fechaObtencion.Year;
        if (fechaObtencion.AddYears(anios) > fechaReferencia)
        {
            anios--;
        }

        return anios;
    }

    public async Task<TecnicoDto> CreateAsync(CrearTecnicoDto dto)
    {
        var tecnico = new Tecnico
        {
            NombreCompleto = dto.NombreCompleto.Trim(),
            Identificacion = dto.Identificacion.Trim(),
            Telefono = dto.Telefono?.Trim() ?? string.Empty,
            Correo = dto.Correo?.Trim() ?? string.Empty,
            Disponible = dto.Disponible!.Value,
            TarifaPorHora = dto.TarifaPorHora!.Value
        };

        var tecnicoCreado = await _repository.AddAsync(tecnico)
            ?? tecnico;

        return MapToDto(tecnicoCreado);
    }

    public async Task UpdateAsync(
        int id,
        ActualizarTecnicoDto dto)
    {
        ValidarId(id);
        var tecnico = await _repository.GetByIdAsync(id);

        if (tecnico is null)
        {
            throw new ResourceNotFoundException($"No se encontró el técnico con id {id}.");
        }

        tecnico.NombreCompleto = dto.NombreCompleto.Trim();
        tecnico.Identificacion = dto.Identificacion.Trim();
        tecnico.Telefono = dto.Telefono?.Trim() ?? string.Empty;
        tecnico.Correo = dto.Correo?.Trim() ?? string.Empty;
        tecnico.Disponible = dto.Disponible!.Value;
        tecnico.TarifaPorHora = dto.TarifaPorHora!.Value;

        _repository.Update(tecnico);
    }

    public async Task DeleteAsync(int id)
    {
        ValidarId(id);
        var tecnico = await _repository.GetByIdAsync(id);

        if (tecnico is null)
        {
            throw new ResourceNotFoundException($"No se encontró el técnico con id {id}.");
        }

        if (await _repository.HasAssociatedSpecialtiesAsync(id))
        {
            throw new ConflictException("No se puede eliminar el técnico porque tiene especialidades asociadas.");
        }

        _repository.Delete(id);
    }

    private static void ValidarId(int id)
    {
        if (id <= 0)
            throw new InvalidRequestException("El identificador debe ser mayor que cero.");
    }

    private static TecnicoDto MapToDto(Tecnico tecnico)
    {
        return new TecnicoDto
        {
            Id = tecnico.Id,
            NombreCompleto = tecnico.NombreCompleto,
            Identificacion = tecnico.Identificacion,
            Telefono = tecnico.Telefono,
            Correo = tecnico.Correo,
            Disponible = tecnico.Disponible,
            TarifaPorHora = tecnico.TarifaPorHora
        };
    }
}
