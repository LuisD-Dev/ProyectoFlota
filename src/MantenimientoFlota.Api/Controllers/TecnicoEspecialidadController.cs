using MantenimientoFlota.Application.Common.Results;
using MantenimientoFlota.Application.DTOs.TecnicoEspecialidad;
using MantenimientoFlota.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MantenimientoFlota.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TecnicoEspecialidadController : ControllerBase
{
    private readonly ITecnicoEspecialidadService _service;

    public TecnicoEspecialidadController(ITecnicoEspecialidadService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IEnumerable<TecnicoEspecialidadDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TecnicoEspecialidadDto>>> GetAllAsync()
    {
        var tecnicoEspecialidades = await _service.GetAllAsync();

        return Ok(tecnicoEspecialidades);
    }

    [HttpGet("{tecnicoId:int}/{especialidadId:int}")]
    [ProducesResponseType(typeof(TecnicoEspecialidadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TecnicoEspecialidadDto>> GetById(
        int tecnicoId,
        int especialidadId)
    {
        var tecnicoEspecialidad = await _service.GetByIdAsync(
            tecnicoId,
            especialidadId);

        if (tecnicoEspecialidad is null)
        {
            return NotFound(new
            {
                mensaje = $"No se encontró la relación entre el técnico con id {tecnicoId} y la especialidad con id {especialidadId}."
            });
        }

        return Ok(tecnicoEspecialidad);
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(TecnicoEspecialidadDto),
        StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TecnicoEspecialidadDto>> CreateAsync(
        CrearTecnicoEspecialidadDto dto)
    {
        var result = await _service.CreateAsync(dto);

        if (result.Status ==
            TecnicoEspecialidadOperationStatus.TechnicianDoesNotExist)
        {
            return NotFound(new
            {
                mensaje = $"No se encontró el técnico con id {dto.TecnicoId}."
            });
        }

        if (result.Status ==
            TecnicoEspecialidadOperationStatus.SpecialtyDoesNotExist)
        {
            return NotFound(new
            {
                mensaje = $"No se encontró la especialidad con id {dto.EspecialidadId}."
            });
        }

        if (result.Status ==
            TecnicoEspecialidadOperationStatus.RelationshipAlreadyExists)
        {
            return Conflict(new
            {
                mensaje = "Ya existe una relación entre el técnico y la especialidad indicados."
            });
        }

        var tecnicoEspecialidad = result.Value!;

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                tecnicoId = tecnicoEspecialidad.TecnicoId,
                especialidadId = tecnicoEspecialidad.EspecialidadId
            },
            tecnicoEspecialidad);
    }

    [HttpPut("{tecnicoId:int}/{especialidadId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync(
        int tecnicoId,
        int especialidadId,
        ActualizarTecnicoEspecialidadDto dto)
    {
        var result = await _service.UpdateAsync(
            tecnicoId,
            especialidadId,
            dto);

        if (result.Status == TecnicoEspecialidadOperationStatus.DoesNotExist)
        {
            return NotFound(new
            {
                mensaje = $"No se encontró la relación entre el técnico con id {tecnicoId} y la especialidad con id {especialidadId}."
            });
        }

        return NoContent();
    }

    [HttpDelete("{tecnicoId:int}/{especialidadId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(
        int tecnicoId,
        int especialidadId)
    {
        var result = await _service.DeleteAsync(tecnicoId, especialidadId);

        if (result.Status == TecnicoEspecialidadOperationStatus.DoesNotExist)
        {
            return NotFound(new
            {
                mensaje = $"No se encontró la relación entre el técnico con id {tecnicoId} y la especialidad con id {especialidadId}."
            });
        }

        return NoContent();
    }
}
