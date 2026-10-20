using MantenimientoFlota.Api.Common;
using MantenimientoFlota.Api.Contracts;
using MantenimientoFlota.Application.Common.Results;
using MantenimientoFlota.Application.DTOs.Tecnico;
using MantenimientoFlota.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace MantenimientoFlota.Api.Controllers;

[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
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
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<TecnicoDto>>> GetAllAsync(
        [FromQuery] bool? disponible = null,
        [FromQuery] int? especialidadId = null)
    {
        if (especialidadId is <= 0)
        {
            return BadRequest(ApiErrorFactory.Create(
                HttpContext,
                StatusCodes.Status400BadRequest,
                "El identificador de la especialidad debe ser mayor que cero."));
        }

        var tecnicos = await _service.GetAllAsync(disponible, especialidadId);

        return Ok(tecnicos);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TecnicoDetalleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TecnicoDetalleDto>> GetById(int id)
    {
        if (id <= 0)
        {
            return BadRequest(ApiErrorFactory.Create(HttpContext, StatusCodes.Status400BadRequest, "El identificador debe ser mayor que cero."));
        }

        var tecnico = await _service.GetByIdAsync(id);

        if (tecnico is null)
        {
            return NotFound(ApiErrorFactory.Create(HttpContext, StatusCodes.Status404NotFound, $"No se encontró el técnico con id {id}."));
        }

        return Ok(tecnico);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TecnicoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
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
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync(
        int id,
        ActualizarTecnicoDto dto)
    {
        if (id <= 0)
        {
            return BadRequest(ApiErrorFactory.Create(HttpContext, StatusCodes.Status400BadRequest, "El identificador debe ser mayor que cero."));
        }

        var result = await _service.UpdateAsync(id, dto);

        if (result.Status == TecnicoOperationStatus.DoesNotExist)
        {
            return NotFound(ApiErrorFactory.Create(HttpContext, StatusCodes.Status404NotFound, $"No se encontró el técnico con id {id}."));
        }

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
            return BadRequest(ApiErrorFactory.Create(HttpContext, StatusCodes.Status400BadRequest, "El identificador debe ser mayor que cero."));
        }

        var result = await _service.DeleteAsync(id);

        if (result.Status == TecnicoOperationStatus.DoesNotExist)
        {
            return NotFound(ApiErrorFactory.Create(HttpContext, StatusCodes.Status404NotFound, $"No se encontró el técnico con id {id}."));
        }

        if (result.Status == TecnicoOperationStatus.HasAssociatedSpecialties)
        {
            return Conflict(ApiErrorFactory.Create(HttpContext, StatusCodes.Status409Conflict, "No se puede eliminar el técnico porque tiene especialidades asociadas."));
        }

        return NoContent();
    }
}
