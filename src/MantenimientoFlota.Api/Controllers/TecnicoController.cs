using MantenimientoFlota.Domain.DTO.TecnicoEspecialidad;
using MantenimientoFlota.Domain.DTO.PatternResult;
using MantenimientoFlota.Api.Common;
using System.ComponentModel.DataAnnotations;
using MantenimientoFlota.Api.Contracts;

using MantenimientoFlota.Domain.DTO.Tecnico;
using MantenimientoFlota.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MantenimientoFlota.Api.Controllers;

[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
[ApiController]
[Route("api/[controller]")]
public sealed class TecnicoController : ControllerBase
{
    private readonly ITecnicoService _service;
    private readonly ITecnicoEspecialidadService _relaciones;

    public TecnicoController(ITecnicoService service, ITecnicoEspecialidadService relaciones)
    {
        _service = service;
        _relaciones = relaciones;
    }

    [EndpointSummary("Listar técnicos")]
    [HttpGet]
    [ProducesResponseType(typeof(Result<IEnumerable<TecnicoDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result<IEnumerable<TecnicoDto>>>> GetAllAsync(
        [FromServices] ILogger<TecnicoController> logger,
        [FromQuery] bool? disponible = null,
        [FromQuery] [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int? especialidadId = null)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Tecnico.GetAllAsync");

        var tecnicos = await _service.GetAllAsync(disponible, especialidadId);

        return this.FromResult(tecnicos);
    }

    [EndpointSummary("Consultar detalle de técnicos")]
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Result<TecnicoDetalleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<TecnicoDetalleDto>>> GetById(
        [FromServices] ILogger<TecnicoController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int id)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Tecnico.GetById");

        var tecnico = await _service.GetByIdAsync(id);

        return this.FromResult(tecnico);
    }

    [EndpointSummary("Registrar técnicos")]
    [HttpPost]
    [ProducesResponseType(typeof(Result<TecnicoDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public async Task<ActionResult<Result<TecnicoDto>>> CreateAsync(
        [FromServices] ILogger<TecnicoController> logger,
        CrearTecnicoDto dto)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Tecnico.CreateAsync");
        var result = await _service.CreateAsync(dto);

        if (!result.IsSuccess) return this.FromResult(result);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Data!.Id },
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

        var result = await _service.UpdateAsync(id, dto);

        return this.FromResult(result);
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

        var result = await _service.DeleteAsync(id);

        return this.FromResult(result);
    }

    [EndpointSummary("Listar asociaciones técnico-especialidad")]
    [HttpGet("{tecnicoId:int}/especialidades")]
    [ProducesResponseType(
        typeof(Result<IEnumerable<TecnicoEspecialidadDto>>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<IEnumerable<TecnicoEspecialidadDto>>>> ListarEspecialidadesAsync(
        [FromServices] ILogger<TecnicoController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int tecnicoId)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "TecnicoEspecialidad.ListarEspecialidadesAsync");
        var tecnicoEspecialidades = await _relaciones.GetAllAsync(tecnicoId);

        return this.FromResult(tecnicoEspecialidades);
    }

    [EndpointSummary("Consultar detalle de asociaciones técnico-especialidad")]
    [HttpGet("{tecnicoId:int}/especialidades/{especialidadId:int}")]
    [ProducesResponseType(typeof(Result<TecnicoEspecialidadDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result<TecnicoEspecialidadDto>>> ConsultarEspecialidad(
        [FromServices] ILogger<TecnicoController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int tecnicoId,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int especialidadId)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "TecnicoEspecialidad.ConsultarEspecialidad");
        var tecnicoEspecialidad = await _relaciones.GetByIdAsync(
            tecnicoId,
            especialidadId);

        return this.FromResult(tecnicoEspecialidad);
    }

    [EndpointSummary("Registrar asociaciones técnico-especialidad")]
    [HttpPost("{tecnicoId:int}/especialidades")]
    [ProducesResponseType(
        typeof(Result<TecnicoEspecialidadDto>),
        StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public async Task<ActionResult<Result<TecnicoEspecialidadDto>>> AsociarEspecialidadAsync(
        [FromServices] ILogger<TecnicoController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int tecnicoId,
        CrearTecnicoEspecialidadDto dto)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "TecnicoEspecialidad.AsociarEspecialidadAsync");
        var result = await _relaciones.CreateAsync(tecnicoId, dto);

        var tecnicoEspecialidad = result.Data;

        if (!result.IsSuccess) return this.FromResult(result);

        return CreatedAtAction(
            nameof(ConsultarEspecialidad),
            new
            {
                tecnicoId = tecnicoEspecialidad!.TecnicoId,
                especialidadId = tecnicoEspecialidad!.EspecialidadId
            },
            result);
    }

    [EndpointSummary("Actualizar asociaciones técnico-especialidad")]
    [HttpPut("{tecnicoId:int}/especialidades/{especialidadId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public async Task<IActionResult> ActualizarEspecialidadAsync(
        [FromServices] ILogger<TecnicoController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int tecnicoId,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int especialidadId,
        ActualizarTecnicoEspecialidadDto dto)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "TecnicoEspecialidad.ActualizarEspecialidadAsync");
        var result = await _relaciones.UpdateAsync(
            tecnicoId,
            especialidadId,
            dto);
        return this.FromResult(result);
    }

    [EndpointSummary("Eliminar asociaciones técnico-especialidad")]
    [HttpDelete("{tecnicoId:int}/especialidades/{especialidadId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DesasociarEspecialidadAsync(
        [FromServices] ILogger<TecnicoController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int tecnicoId,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int especialidadId)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "TecnicoEspecialidad.DesasociarEspecialidadAsync");
        var result = await _relaciones.DeleteAsync(tecnicoId, especialidadId);
        return this.FromResult(result);
    }
}
