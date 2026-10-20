using MantenimientoFlota.Api.Contracts;
using MantenimientoFlota.Application.Common.Exceptions;
using MantenimientoFlota.Application.DTOs.Especialidad;
using MantenimientoFlota.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace MantenimientoFlota.Api.Controllers;

[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
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
    [ProducesResponseType(typeof(EspecialidadDetalleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EspecialidadDetalleDto>> GetById(int id)
    {
        if (id <= 0)
        {
            throw new InvalidRequestException("El identificador debe ser mayor que cero.");
        }

        var especialidad = await _service.GetByIdAsync(id);

        return Ok(especialidad);
    }

    [HttpPost]
    [ProducesResponseType(typeof(EspecialidadDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<EspecialidadDto>> CreateAsync(
        CrearEspecialidadDto dto)
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
    public async Task<IActionResult> UpdateAsync(
        int id,
        ActualizarEspecialidadDto dto)
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
