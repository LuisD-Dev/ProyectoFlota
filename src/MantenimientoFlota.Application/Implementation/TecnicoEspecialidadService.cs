using MantenimientoFlota.Application.Common.Exceptions;
using MantenimientoFlota.Application.DTOs.TecnicoEspecialidad;
using MantenimientoFlota.Application.Interfaces.Services;
using MantenimientoFlota.Application.Interfaces.Repositories;
using MantenimientoFlota.Domain.Entities;

namespace MantenimientoFlota.Application.Implementation;

public class TecnicoEspecialidadService : ITecnicoEspecialidadService
{
    private readonly ITecnicoEspecialidadRepository _repository;

    public TecnicoEspecialidadService(
        ITecnicoEspecialidadRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<TecnicoEspecialidadDto>> GetAllAsync()
    {
        var tecnicoEspecialidades = await _repository.GetAllAsync();

        return tecnicoEspecialidades
            .Select(MapToDto)
            .ToList();
    }

    public async Task<TecnicoEspecialidadDto> GetByIdAsync(
        int tecnicoId,
        int especialidadId)
    {
        var tecnicoEspecialidad = await _repository.GetByIdAsync(
            tecnicoId,
            especialidadId);

        return tecnicoEspecialidad is null
            ? throw new ResourceNotFoundException($"No se encontró la relación entre el técnico con id {tecnicoId} y la especialidad con id {especialidadId}.")
            : MapToDto(tecnicoEspecialidad);
    }

    public async Task<TecnicoEspecialidadDto> CreateAsync(
        CrearTecnicoEspecialidadDto dto)
    {
        var tecnicoId = dto.TecnicoId!.Value;
        var especialidadId = dto.EspecialidadId!.Value;

        if (!await _repository.TechnicianExistsAsync(tecnicoId))
        {
            throw new ResourceNotFoundException($"No se encontró el técnico con id {dto.TecnicoId}.");
        }

        if (!await _repository.SpecialtyExistsAsync(especialidadId))
        {
            throw new ResourceNotFoundException($"No se encontró la especialidad con id {dto.EspecialidadId}.");
        }

        if (await _repository.ExistsAsync(tecnicoId, especialidadId))
        {
            throw new ConflictException("Ya existe una relación entre el técnico y la especialidad indicados.");
        }

        var tecnicoEspecialidad = new TecnicoEspecialidad
        {
            TecnicoId = tecnicoId,
            EspecialidadId = especialidadId,
            NivelCertificacion = dto.NivelCertificacion.Trim(),
            FechaObtencion = dto.FechaObtencion!.Value
        };

        var relacionCreada = await _repository.AddAsync(tecnicoEspecialidad);
        var relacionCompleta = await _repository.GetByIdAsync(
            relacionCreada.TecnicoId,
            relacionCreada.EspecialidadId)
            ?? relacionCreada;

        return MapToDto(relacionCompleta);
    }

    public async Task UpdateAsync(
        int tecnicoId,
        int especialidadId,
        ActualizarTecnicoEspecialidadDto dto)
    {
        if (!await _repository.ExistsAsync(tecnicoId, especialidadId))
        {
            throw new ResourceNotFoundException($"No se encontró la relación entre el técnico con id {tecnicoId} y la especialidad con id {especialidadId}.");
        }

        var tecnicoEspecialidad = new TecnicoEspecialidad
        {
            TecnicoId = tecnicoId,
            EspecialidadId = especialidadId,
            NivelCertificacion = dto.NivelCertificacion.Trim(),
            FechaObtencion = dto.FechaObtencion!.Value
        };

        await _repository.UpdateAsync(tecnicoEspecialidad);

    }

    public async Task DeleteAsync(
        int tecnicoId,
        int especialidadId)
    {
        if (!await _repository.ExistsAsync(tecnicoId, especialidadId))
        {
            throw new ResourceNotFoundException($"No se encontró la relación entre el técnico con id {tecnicoId} y la especialidad con id {especialidadId}.");
        }

        await _repository.DeleteAsync(tecnicoId, especialidadId);

    }

    private static TecnicoEspecialidadDto MapToDto(
        TecnicoEspecialidad tecnicoEspecialidad)
    {
        return new TecnicoEspecialidadDto
        {
            TecnicoId = tecnicoEspecialidad.TecnicoId,
            TecnicoNombreCompleto = tecnicoEspecialidad.Tecnico?.NombreCompleto
                ?? string.Empty,
            EspecialidadId = tecnicoEspecialidad.EspecialidadId,
            EspecialidadNombre = tecnicoEspecialidad.Especialidad?.Nombre
                ?? string.Empty,
            NivelCertificacion = tecnicoEspecialidad.NivelCertificacion,
            FechaObtencion = tecnicoEspecialidad.FechaObtencion
        };
    }
}
