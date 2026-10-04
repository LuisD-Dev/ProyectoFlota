using MantenimientoFlota.Application.Common.Results;
using MantenimientoFlota.Application.DTOs.Tecnico;
using MantenimientoFlota.Application.Interfaces;
using MantenimientoFlota.Domain.Entities;

namespace MantenimientoFlota.Application.Implementation;

public class TecnicoService : ITecnicoService
{
    private readonly ITecnicoRepository _repository;

    public TecnicoService(ITecnicoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TecnicoDto>> GetAllAsync()
    {
        var tecnicos = await _repository.GetAllAsync();

        return tecnicos
            .Select(MapToDto)
            .ToList();
    }

    public async Task<TecnicoDto?> GetByIdAsync(int id)
    {
        var tecnico = await _repository.GetByIdAsync(id);

        return tecnico is null
            ? null
            : MapToDto(tecnico);
    }

    public async Task<TecnicoOperationResult> CreateAsync(CrearTecnicoDto dto)
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

        return new(
            TecnicoOperationStatus.Succeeded,
            MapToDto(tecnicoCreado));
    }

    public async Task<TecnicoOperationResult> UpdateAsync(
        int id,
        ActualizarTecnicoDto dto)
    {
        var tecnico = await _repository.GetByIdAsync(id);

        if (tecnico is null)
        {
            return new(TecnicoOperationStatus.DoesNotExist);
        }

        tecnico.NombreCompleto = dto.NombreCompleto.Trim();
        tecnico.Identificacion = dto.Identificacion.Trim();
        tecnico.Telefono = dto.Telefono?.Trim() ?? string.Empty;
        tecnico.Correo = dto.Correo?.Trim() ?? string.Empty;
        tecnico.Disponible = dto.Disponible!.Value;
        tecnico.TarifaPorHora = dto.TarifaPorHora!.Value;

        _repository.Update(tecnico);

        return new(TecnicoOperationStatus.Succeeded);
    }

    public async Task<TecnicoOperationResult> DeleteAsync(int id)
    {
        var tecnico = await _repository.GetByIdAsync(id);

        if (tecnico is null)
        {
            return new(TecnicoOperationStatus.DoesNotExist);
        }

        if (await _repository.HasAssociatedSpecialtiesAsync(id))
        {
            return new(TecnicoOperationStatus.HasAssociatedSpecialties);
        }

        _repository.Delete(id);

        return new(TecnicoOperationStatus.Succeeded);
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
