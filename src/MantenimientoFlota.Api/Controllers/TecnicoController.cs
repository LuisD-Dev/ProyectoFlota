using System.ComponentModel.DataAnnotations;
using MantenimientoFlota.Api.Contracts;

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

    [EndpointSummary("Listar técnicos")]
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TecnicoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<TecnicoDto>>> GetAllAsync(
        [FromServices] ILogger<TecnicoController> logger,
        [FromQuery] bool? disponible = null,
        [FromQuery] [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int? especialidadId = null)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Tecnico.GetAllAsync");

        var tecnicos = await _service.GetAllAsync(disponible, especialidadId);

        return Ok(tecnicos);
    }

    [EndpointSummary("Consultar detalle de técnicos")]
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TecnicoDetalleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TecnicoDetalleDto>> GetById(
        [FromServices] ILogger<TecnicoController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int id)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Tecnico.GetById");

        var tecnico = await _service.GetByIdAsync(id);

        return Ok(tecnico);
    }

    [EndpointSummary("Registrar técnicos")]
    [HttpPost]
    [ProducesResponseType(typeof(TecnicoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public async Task<ActionResult<TecnicoDto>> CreateAsync(
        [FromServices] ILogger<TecnicoController> logger,
        CrearTecnicoDto dto)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Tecnico.CreateAsync");
        var result = await _service.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
    }

    [EndpointSummary("Actualizar técnicos")]
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public async Task<IActionResult> UpdateAsync(
        [FromServices] ILogger<TecnicoController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int id,
        ActualizarTecnicoDto dto)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Tecnico.UpdateAsync");

        await _service.UpdateAsync(id, dto);

        return NoContent();
    }

    [EndpointSummary("Eliminar técnicos")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteAsync(
        [FromServices] ILogger<TecnicoController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int id)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Tecnico.DeleteAsync");

        await _service.DeleteAsync(id);

        return NoContent();
    }
}
