using MantenimientoFlota.Application.Common.Results;
using MantenimientoFlota.Application.DTOs.Activo;
using MantenimientoFlota.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MantenimientoFlota.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ActivoController : ControllerBase
{
    private readonly IActivoService _service;

    public ActivoController(IActivoService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ActivoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ActivoDto>>> GetAllAsync()
    {
        var activos = await _service.GetAllAsync();

        return Ok(activos);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ActivoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ActivoDto>> GetById(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                mensaje = "El identificador debe ser mayor que cero."
            });
        }

        var activo = await _service.GetByIdAsync(id);

        if (activo is null)
        {
            return NotFound(new
            {
                mensaje = $"No se encontró el activo con id {id}."
            });
        }

        return Ok(activo);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ActivoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ActivoDto>> CreateAsync(CrearActivoDto dto)
    {
        var result = await _service.CreateAsync(dto);

        if (result.Status == ActivoOperationStatus.CodeAlreadyExists)
        {
            return Conflict(new
            {
                mensaje = "Ya existe un activo con el código indicado."
            });
        }

        if (result.Status == ActivoOperationStatus.CategoryDoesNotExist)
        {
            return BadRequest(new
            {
                mensaje = "No existe una categoría de activo con el identificador indicado."
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
        ActualizarActivoDto dto)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                mensaje = "El identificador debe ser mayor que cero."
            });
        }

        var result = await _service.UpdateAsync(id, dto);

        if (result.Status == ActivoOperationStatus.DoesNotExist)
        {
            return NotFound(new
            {
                mensaje = $"No se encontró el activo con id {id}."
            });
        }

        if (result.Status == ActivoOperationStatus.CodeAlreadyExists)
        {
            return Conflict(new
            {
                mensaje = "Ya existe un activo con el código indicado."
            });
        }

        if (result.Status == ActivoOperationStatus.CategoryDoesNotExist)
        {
            return BadRequest(new
            {
                mensaje = "No existe una categoría de activo con el identificador indicado."
            });
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        if (result.Status == ActivoOperationStatus.DoesNotExist)
        {
            return NotFound(new
            {
                mensaje = $"No se encontró el activo con id {id}."
            });
        }

        return NoContent();
    }
}
