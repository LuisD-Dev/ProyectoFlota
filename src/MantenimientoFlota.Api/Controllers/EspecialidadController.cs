using MantenimientoFlota.Application.Common.Results;
using MantenimientoFlota.Application.DTOs.Especialidad;
using MantenimientoFlota.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MantenimientoFlota.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class EspecialidadController : ControllerBase
{
    private readonly IEspecialidadService _service;

    public EspecialidadController(IEspecialidadService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IEnumerable<EspecialidadDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EspecialidadDto>>> GetAllAsync()
    {
        var especialidades = await _service.GetAllAsync();

        return Ok(especialidades);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EspecialidadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EspecialidadDto>> GetById(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                mensaje = "El identificador debe ser mayor que cero."
            });
        }

        var especialidad = await _service.GetByIdAsync(id);

        if (especialidad is null)
        {
            return NotFound(new
            {
                mensaje = $"No se encontró la especialidad con id {id}."
            });
        }

        return Ok(especialidad);
    }

    [HttpPost]
    [ProducesResponseType(typeof(EspecialidadDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<EspecialidadDto>> CreateAsync(
        CrearEspecialidadDto dto)
    {
        var result = await _service.CreateAsync(dto);

        if (result.Status == EspecialidadOperationStatus.NameAlreadyExists)
        {
            return Conflict(new
            {
                mensaje = "Ya existe una especialidad con el nombre indicado."
            });
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value!.Id },
            result.Value);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateAsync(
        int id,
        ActualizarEspecialidadDto dto)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                mensaje = "El identificador debe ser mayor que cero."
            });
        }

        var result = await _service.UpdateAsync(id, dto);

        if (result.Status == EspecialidadOperationStatus.DoesNotExist)
        {
            return NotFound(new
            {
                mensaje = $"No se encontró la especialidad con id {id}."
            });
        }

        if (result.Status == EspecialidadOperationStatus.NameAlreadyExists)
        {
            return Conflict(new
            {
                mensaje = "Ya existe una especialidad con el nombre indicado."
            });
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                mensaje = "El identificador debe ser mayor que cero."
            });
        }

        var result = await _service.DeleteAsync(id);

        if (result.Status == EspecialidadOperationStatus.DoesNotExist)
        {
            return NotFound(new
            {
                mensaje = $"No se encontró la especialidad con id {id}."
            });
        }

        if (result.Status == EspecialidadOperationStatus.HasAssociatedTechnicians)
        {
            return Conflict(new
            {
                mensaje = "No se puede eliminar la especialidad porque tiene técnicos asociados."
            });
        }

        return NoContent();
    }
}
