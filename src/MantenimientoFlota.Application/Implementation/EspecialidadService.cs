using MantenimientoFlota.Application.Common.Results;
using MantenimientoFlota.Application.DTOs.Especialidad;
using MantenimientoFlota.Application.Interfaces;
using MantenimientoFlota.Domain.Entities;

namespace MantenimientoFlota.Application.Implementation;

public class EspecialidadService : IEspecialidadService
{
    private readonly IEspecialidadRepository _repository;

    public EspecialidadService(IEspecialidadRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<EspecialidadDto>> GetAllAsync()
    {
        var especialidades = await _repository.GetAllAsync();

        return especialidades
            .Select(MapToDto)
            .ToList();
    }

    public async Task<EspecialidadDto?> GetByIdAsync(int id)
    {
        var especialidad = await _repository.GetByIdAsync(id);

        return especialidad is null
            ? null
            : MapToDto(especialidad);
    }

    public async Task<EspecialidadOperationResult> CreateAsync(
        CrearEspecialidadDto dto)
    {
        var nombre = dto.Nombre.Trim();

        if (await _repository.ExistsByNameAsync(nombre))
        {
            return new(EspecialidadOperationStatus.NameAlreadyExists);
        }

        var especialidad = new Especialidad
        {
            Nombre = nombre,
            Descripcion = dto.Descripcion ?? string.Empty
        };

        var especialidadCreada = await _repository.AddAsync(especialidad);

        return new(
            EspecialidadOperationStatus.Succeeded,
            MapToDto(especialidadCreada));
    }

    public async Task<EspecialidadOperationResult> UpdateAsync(
        int id,
        ActualizarEspecialidadDto dto)
    {
        var especialidad = await _repository.GetByIdAsync(id);

        if (especialidad is null)
        {
            return new(EspecialidadOperationStatus.DoesNotExist);
        }

        var nombre = dto.Nombre.Trim();

        if (await _repository.ExistsByNameAsync(nombre, id))
        {
            return new(EspecialidadOperationStatus.NameAlreadyExists);
        }

        especialidad.Nombre = nombre;
        especialidad.Descripcion = dto.Descripcion ?? string.Empty;

        await _repository.UpdateAsync(especialidad);

        return new(EspecialidadOperationStatus.Succeeded);
    }

    public async Task<EspecialidadOperationResult> DeleteAsync(int id)
    {
        var especialidad = await _repository.GetByIdAsync(id);

        if (especialidad is null)
        {
            return new(EspecialidadOperationStatus.DoesNotExist);
        }

        if (await _repository.HasAssociatedTechniciansAsync(id))
        {
            return new(EspecialidadOperationStatus.HasAssociatedTechnicians);
        }

        await _repository.DeleteAsync(especialidad);

        return new(EspecialidadOperationStatus.Succeeded);
    }

    private static EspecialidadDto MapToDto(Especialidad especialidad)
    {
        return new EspecialidadDto
        {
            Id = especialidad.Id,
            Nombre = especialidad.Nombre,
            Descripcion = especialidad.Descripcion
        };
    }
}
