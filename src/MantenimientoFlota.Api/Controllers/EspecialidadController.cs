using MantenimientoFlota.Domain.DTO.PatternResult;
using MantenimientoFlota.Api.Common;
using System.ComponentModel.DataAnnotations;
using MantenimientoFlota.Api.Contracts;

using MantenimientoFlota.Domain.DTO.Especialidad;
using MantenimientoFlota.Application.Interfaces;
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
        typeof(Result<IEnumerable<EspecialidadDto>>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<IEnumerable<EspecialidadDto>>>> GetAllAsync(
        [FromServices] ILogger<EspecialidadController> logger)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Especialidad.GetAllAsync");
        var especialidades = await _service.GetAllAsync();

        return this.FromResult(especialidades);
    }

    [EndpointSummary("Consultar detalle de especialidades")]
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Result<EspecialidadDetalleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<EspecialidadDetalleDto>>> GetById(
        [FromServices] ILogger<EspecialidadController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int id)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Especialidad.GetById");

        var especialidad = await _service.GetByIdAsync(id);

        return this.FromResult(especialidad);
    }

    [EndpointSummary("Registrar especialidades")]
    [HttpPost]
    [ProducesResponseType(typeof(Result<EspecialidadDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public async Task<ActionResult<Result<EspecialidadDto>>> CreateAsync(
        [FromServices] ILogger<EspecialidadController> logger,
        CrearEspecialidadDto dto)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Especialidad.CreateAsync");
        var result = await _service.CreateAsync(dto);

        if (!result.IsSuccess) return this.FromResult(result);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Data!.Id },
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

        var result = await _service.UpdateAsync(id, dto);

        return this.FromResult(result);
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

        var result = await _service.DeleteAsync(id);

        return this.FromResult(result);
    }
}
