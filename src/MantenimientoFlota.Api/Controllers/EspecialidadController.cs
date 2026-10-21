using System.ComponentModel.DataAnnotations;
using MantenimientoFlota.Api.Contracts;

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

    [EndpointSummary("Listar especialidades")]
    [HttpGet]
    [ProducesResponseType(
        typeof(IEnumerable<EspecialidadDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EspecialidadDto>>> GetAllAsync(
        [FromServices] ILogger<EspecialidadController> logger)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Especialidad.GetAllAsync");
        var especialidades = await _service.GetAllAsync();

        return Ok(especialidades);
    }

    [EndpointSummary("Consultar detalle de especialidades")]
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EspecialidadDetalleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EspecialidadDetalleDto>> GetById(
        [FromServices] ILogger<EspecialidadController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int id)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Especialidad.GetById");

        var especialidad = await _service.GetByIdAsync(id);

        return Ok(especialidad);
    }

    [EndpointSummary("Registrar especialidades")]
    [HttpPost]
    [ProducesResponseType(typeof(EspecialidadDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public async Task<ActionResult<EspecialidadDto>> CreateAsync(
        [FromServices] ILogger<EspecialidadController> logger,
        CrearEspecialidadDto dto)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Especialidad.CreateAsync");
        var result = await _service.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
    }

    [EndpointSummary("Actualizar especialidades")]
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public async Task<IActionResult> UpdateAsync(
        [FromServices] ILogger<EspecialidadController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int id,
        ActualizarEspecialidadDto dto)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Especialidad.UpdateAsync");

        await _service.UpdateAsync(id, dto);

        return NoContent();
    }

    [EndpointSummary("Eliminar especialidades")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteAsync(
        [FromServices] ILogger<EspecialidadController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int id)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Especialidad.DeleteAsync");

        await _service.DeleteAsync(id);

        return NoContent();
    }
}
