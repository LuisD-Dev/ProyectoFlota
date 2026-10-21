using System.ComponentModel.DataAnnotations;
using MantenimientoFlota.Api.Contracts;

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

    [EndpointSummary("Listar repuestos")]
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RepuestoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<RepuestoDto>>> GetAllAsync(
        [FromServices] ILogger<RepuestoController> logger,
        [FromQuery] bool stockBajo = false)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Repuesto.GetAllAsync");
        var repuestos = await _service.GetAllAsync(stockBajo);

        return Ok(repuestos);
    }

    [EndpointSummary("Consultar detalle de repuestos")]
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(RepuestoDetalleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RepuestoDetalleDto>> GetById(
        [FromServices] ILogger<RepuestoController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int id)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Repuesto.GetById");

        var repuesto = await _service.GetByIdAsync(id);

        return Ok(repuesto);
    }

    [EndpointSummary("Registrar repuestos")]
    [HttpPost]
    [ProducesResponseType(typeof(RepuestoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public async Task<ActionResult<RepuestoDto>> CreateAsync(
        [FromServices] ILogger<RepuestoController> logger,
        CrearRepuestoDto dto)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Repuesto.CreateAsync");
        var result = await _service.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
    }

    [EndpointSummary("Actualizar repuestos")]
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public async Task<IActionResult> UpdateAsync(
        [FromServices] ILogger<RepuestoController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int id, ActualizarRepuestoDto dto)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Repuesto.UpdateAsync");

        await _service.UpdateAsync(id, dto);

        return NoContent();
    }

    [EndpointSummary("Eliminar repuestos")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteAsync(
        [FromServices] ILogger<RepuestoController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int id)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Repuesto.DeleteAsync");

        await _service.DeleteAsync(id);

        return NoContent();
    }
}
