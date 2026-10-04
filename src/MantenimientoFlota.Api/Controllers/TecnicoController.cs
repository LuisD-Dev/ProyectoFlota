using MantenimientoFlota.Application.Common.Results;
using MantenimientoFlota.Application.DTOs.Tecnico;
using MantenimientoFlota.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MantenimientoFlota.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TecnicoController : ControllerBase
{
    private readonly ITecnicoService _service;

    public TecnicoController(ITecnicoService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TecnicoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TecnicoDto>>> GetAllAsync()
    {
        var tecnicos = await _service.GetAllAsync();

        return Ok(tecnicos);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TecnicoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TecnicoDto>> GetById(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                mensaje = "El identificador debe ser mayor que cero."
            });
        }

        var tecnico = await _service.GetByIdAsync(id);

        if (tecnico is null)
        {
            return NotFound(new
            {
                mensaje = $"No se encontró el técnico con id {id}."
            });
        }

        return Ok(tecnico);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TecnicoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TecnicoDto>> CreateAsync(CrearTecnicoDto dto)
    {
        var result = await _service.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value!.Id },
            result.Value);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync(
        int id,
        ActualizarTecnicoDto dto)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                mensaje = "El identificador debe ser mayor que cero."
            });
        }

        var result = await _service.UpdateAsync(id, dto);

        if (result.Status == TecnicoOperationStatus.DoesNotExist)
        {
            return NotFound(new
            {
                mensaje = $"No se encontró el técnico con id {id}."
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

        if (result.Status == TecnicoOperationStatus.DoesNotExist)
        {
            return NotFound(new
            {
                mensaje = $"No se encontró el técnico con id {id}."
            });
        }

        if (result.Status == TecnicoOperationStatus.HasAssociatedSpecialties)
        {
            return Conflict(new
            {
                mensaje = "No se puede eliminar el técnico porque tiene especialidades asociadas."
            });
        }

        return NoContent();
    }
}
