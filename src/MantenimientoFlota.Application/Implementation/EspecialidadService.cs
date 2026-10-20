using MantenimientoFlota.Application.Common.Exceptions;
using MantenimientoFlota.Application.DTOs.Especialidad;
using MantenimientoFlota.Application.Interfaces.Services;
using MantenimientoFlota.Application.Interfaces.Repositories;
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

    public async Task<EspecialidadDetalleDto> GetByIdAsync(int id)
    {
        ValidarId(id);
        return await _repository.GetDetalleByIdAsync(id)
            ?? throw new ResourceNotFoundException($"No se encontró la especialidad con id {id}.");
    }

    public async Task<EspecialidadDto> CreateAsync(
        CrearEspecialidadDto dto)
    {
        var nombre = dto.Nombre.Trim();

        if (await _repository.ExistsByNameAsync(nombre))
        {
            throw new ConflictException("Ya existe una especialidad con el nombre indicado.");
        }

        var especialidad = new Especialidad
        {
            Nombre = nombre,
            Descripcion = dto.Descripcion ?? string.Empty
        };

        var especialidadCreada = await _repository.AddAsync(especialidad);

        return MapToDto(especialidadCreada);
    }

    public async Task UpdateAsync(
        int id,
        ActualizarEspecialidadDto dto)
    {
        ValidarId(id);
        var especialidad = await _repository.GetByIdAsync(id);

        if (especialidad is null)
        {
            throw new ResourceNotFoundException($"No se encontró la especialidad con id {id}.");
        }

        var nombre = dto.Nombre.Trim();

        if (await _repository.ExistsByNameAsync(nombre, id))
        {
            throw new ConflictException("Ya existe una especialidad con el nombre indicado.");
        }

        especialidad.Nombre = nombre;
        especialidad.Descripcion = dto.Descripcion ?? string.Empty;

        await _repository.UpdateAsync(especialidad);
    }

    public async Task DeleteAsync(int id)
    {
        ValidarId(id);
        var especialidad = await _repository.GetByIdAsync(id);

        if (especialidad is null)
        {
            throw new ResourceNotFoundException($"No se encontró la especialidad con id {id}.");
        }

        if (await _repository.HasAssociatedTechniciansAsync(id))
        {
            throw new ConflictException("No se puede eliminar la especialidad porque tiene técnicos asociados.");
        }

        await _repository.DeleteAsync(especialidad);
    }

    private static void ValidarId(int id)
    {
        if (id <= 0)
            throw new InvalidRequestException("El identificador debe ser mayor que cero.");
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
