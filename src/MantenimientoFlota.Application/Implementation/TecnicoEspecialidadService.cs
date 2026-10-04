using MantenimientoFlota.Application.Common.Results;
using MantenimientoFlota.Application.DTOs.TecnicoEspecialidad;
using MantenimientoFlota.Application.Interfaces;
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

    public async Task<TecnicoEspecialidadDto?> GetByIdAsync(
        int tecnicoId,
        int especialidadId)
    {
        var tecnicoEspecialidad = await _repository.GetByIdAsync(
            tecnicoId,
            especialidadId);

        return tecnicoEspecialidad is null
            ? null
            : MapToDto(tecnicoEspecialidad);
    }

    public async Task<TecnicoEspecialidadOperationResult> CreateAsync(
        CrearTecnicoEspecialidadDto dto)
    {
        var tecnicoId = dto.TecnicoId!.Value;
        var especialidadId = dto.EspecialidadId!.Value;

        if (!await _repository.TechnicianExistsAsync(tecnicoId))
        {
            return new(
                TecnicoEspecialidadOperationStatus.TechnicianDoesNotExist);
        }

        if (!await _repository.SpecialtyExistsAsync(especialidadId))
        {
            return new(
                TecnicoEspecialidadOperationStatus.SpecialtyDoesNotExist);
        }

        if (await _repository.ExistsAsync(tecnicoId, especialidadId))
        {
            return new(
                TecnicoEspecialidadOperationStatus.RelationshipAlreadyExists);
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

        return new(
            TecnicoEspecialidadOperationStatus.Succeeded,
            MapToDto(relacionCompleta));
    }

    public async Task<TecnicoEspecialidadOperationResult> UpdateAsync(
        int tecnicoId,
        int especialidadId,
        ActualizarTecnicoEspecialidadDto dto)
    {
        if (!await _repository.ExistsAsync(tecnicoId, especialidadId))
        {
            return new(TecnicoEspecialidadOperationStatus.DoesNotExist);
        }

        var tecnicoEspecialidad = new TecnicoEspecialidad
        {
            TecnicoId = tecnicoId,
            EspecialidadId = especialidadId,
            NivelCertificacion = dto.NivelCertificacion.Trim(),
            FechaObtencion = dto.FechaObtencion!.Value
        };

        await _repository.UpdateAsync(tecnicoEspecialidad);

        return new(TecnicoEspecialidadOperationStatus.Succeeded);
    }

    public async Task<TecnicoEspecialidadOperationResult> DeleteAsync(
        int tecnicoId,
        int especialidadId)
    {
        if (!await _repository.ExistsAsync(tecnicoId, especialidadId))
        {
            return new(TecnicoEspecialidadOperationStatus.DoesNotExist);
        }

        await _repository.DeleteAsync(tecnicoId, especialidadId);

        return new(TecnicoEspecialidadOperationStatus.Succeeded);
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
