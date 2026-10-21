using System.ComponentModel.DataAnnotations;
using MantenimientoFlota.Api.Contracts;

using MantenimientoFlota.Application.DTOs.TecnicoEspecialidad;
using MantenimientoFlota.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace MantenimientoFlota.Api.Controllers;

[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
[ApiController]
[Route("api/[controller]")]
public sealed class TecnicoEspecialidadController : ControllerBase
{
    private readonly ITecnicoEspecialidadService _service;

    public TecnicoEspecialidadController(ITecnicoEspecialidadService service)
    {
        _service = service;
    }

    [EndpointSummary("Listar asociaciones técnico-especialidad")]
    [HttpGet]
    [ProducesResponseType(
        typeof(IEnumerable<TecnicoEspecialidadDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TecnicoEspecialidadDto>>> GetAllAsync(
        [FromServices] ILogger<TecnicoEspecialidadController> logger)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "TecnicoEspecialidad.GetAllAsync");
        var tecnicoEspecialidades = await _service.GetAllAsync();

        return Ok(tecnicoEspecialidades);
    }

    [EndpointSummary("Consultar detalle de asociaciones técnico-especialidad")]
    [HttpGet("{tecnicoId:int}/{especialidadId:int}")]
    [ProducesResponseType(typeof(TecnicoEspecialidadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TecnicoEspecialidadDto>> GetById(
        [FromServices] ILogger<TecnicoEspecialidadController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int tecnicoId,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int especialidadId)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "TecnicoEspecialidad.GetById");
        var tecnicoEspecialidad = await _service.GetByIdAsync(
            tecnicoId,
            especialidadId);

        return Ok(tecnicoEspecialidad);
    }

    [EndpointSummary("Registrar asociaciones técnico-especialidad")]
    [HttpPost]
    [ProducesResponseType(
        typeof(TecnicoEspecialidadDto),
        StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public async Task<ActionResult<TecnicoEspecialidadDto>> CreateAsync(
        [FromServices] ILogger<TecnicoEspecialidadController> logger,
        CrearTecnicoEspecialidadDto dto)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "TecnicoEspecialidad.CreateAsync");
        var result = await _service.CreateAsync(dto);

        var tecnicoEspecialidad = result;

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                tecnicoId = tecnicoEspecialidad.TecnicoId,
                especialidadId = tecnicoEspecialidad.EspecialidadId
            },
            tecnicoEspecialidad);
    }

    [EndpointSummary("Actualizar asociaciones técnico-especialidad")]
    [HttpPut("{tecnicoId:int}/{especialidadId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public async Task<IActionResult> UpdateAsync(
        [FromServices] ILogger<TecnicoEspecialidadController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int tecnicoId,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int especialidadId,
        ActualizarTecnicoEspecialidadDto dto)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "TecnicoEspecialidad.UpdateAsync");
        await _service.UpdateAsync(
            tecnicoId,
            especialidadId,
            dto);

        return NoContent();
    }

    [EndpointSummary("Eliminar asociaciones técnico-especialidad")]
    [HttpDelete("{tecnicoId:int}/{especialidadId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteAsync(
        [FromServices] ILogger<TecnicoEspecialidadController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int tecnicoId,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int especialidadId)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "TecnicoEspecialidad.DeleteAsync");
        await _service.DeleteAsync(tecnicoId, especialidadId);

        return NoContent();
    }
}
