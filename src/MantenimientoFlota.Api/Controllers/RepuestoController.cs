using MantenimientoFlota.Application.Common.Results;
using MantenimientoFlota.Application.DTOs.Repuesto;
using MantenimientoFlota.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MantenimientoFlota.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class RepuestoController : ControllerBase
{
    private readonly IRepuestoService _service;

    public RepuestoController(IRepuestoService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RepuestoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RepuestoDto>>> GetAllAsync()
    {
        var repuestos = await _service.GetAllAsync();

        return Ok(repuestos);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(RepuestoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RepuestoDto>> GetById(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                mensaje = "El identificador debe ser mayor que cero."
            });
        }

        var repuesto = await _service.GetByIdAsync(id);

        if (repuesto is null)
        {
            return NotFound(new
            {
                mensaje = $"No se encontró el repuesto con id {id}."
            });
        }

        return Ok(repuesto);
    }

    [HttpPost]
    [ProducesResponseType(typeof(RepuestoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RepuestoDto>> CreateAsync(CrearRepuestoDto dto)
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
    public async Task<IActionResult> UpdateAsync(int id, ActualizarRepuestoDto dto)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                mensaje = "El identificador debe ser mayor que cero."
            });
        }

        var result = await _service.UpdateAsync(id, dto);

        if (result.Status == RepuestoOperationStatus.DoesNotExist)
        {
            return NotFound(new
            {
                mensaje = $"No se encontró el repuesto con id {id}."
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

        if (result.Status == RepuestoOperationStatus.DoesNotExist)
        {
            return NotFound(new
            {
                mensaje = $"No se encontró el repuesto con id {id}."
            });
        }

        if (result.Status == RepuestoOperationStatus.HasAssociatedSuppliers)
        {
            return Conflict(new
            {
                mensaje = "No se puede eliminar el repuesto porque tiene proveedores asociados."
            });
        }

        return NoContent();
    }
}
