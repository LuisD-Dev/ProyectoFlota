using MantenimientoFlota.Api.Contracts;
using MantenimientoFlota.Application.Common.Exceptions;
using MantenimientoFlota.Application.DTOs.Repuesto;
using MantenimientoFlota.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace MantenimientoFlota.Api.Controllers;

[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
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
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<RepuestoDto>>> GetAllAsync(
        [FromQuery] bool stockBajo = false)
    {
        var repuestos = await _service.GetAllAsync(stockBajo);

        return Ok(repuestos);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(RepuestoDetalleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RepuestoDetalleDto>> GetById(int id)
    {
        if (id <= 0)
        {
            throw new InvalidRequestException("El identificador debe ser mayor que cero.");
        }

        var repuesto = await _service.GetByIdAsync(id);

        return Ok(repuesto);
    }

    [HttpPost]
    [ProducesResponseType(typeof(RepuestoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RepuestoDto>> CreateAsync(CrearRepuestoDto dto)
    {
        var result = await _service.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateAsync(int id, ActualizarRepuestoDto dto)
    {
        if (id <= 0)
        {
            throw new InvalidRequestException("El identificador debe ser mayor que cero.");
        }

        await _service.UpdateAsync(id, dto);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        if (id <= 0)
        {
            throw new InvalidRequestException("El identificador debe ser mayor que cero.");
        }

        await _service.DeleteAsync(id);

        return NoContent();
    }
}
