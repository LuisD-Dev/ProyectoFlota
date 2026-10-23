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

    /// <summary>Listar técnicos.</summary>
    /// <param name="logger">Registrador de la operación, resuelto por inyección de dependencias.</param>
    /// <param name="disponible">Parámetro de la operación.</param>
    /// <param name="especialidadId">Identificador positivo del recurso.</param>
    /// <response code="200">Operación completada; el cuerpo contiene Result.</response>
    /// <response code="400">Entrada inválida; consulte los errores por campo.</response>
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

    /// <summary>Consultar detalle de técnicos.</summary>
    /// <param name="logger">Registrador de la operación, resuelto por inyección de dependencias.</param>
    /// <param name="id">Identificador positivo del recurso.</param>
    /// <response code="200">Operación completada; el cuerpo contiene Result.</response>
    /// <response code="400">Entrada inválida; consulte los errores por campo.</response>
    /// <response code="404">El recurso solicitado no existe.</response>
    [EndpointSummary("Consultar detalle de técnicos")]
    [HttpGet("{id:int}", Name = "Tecnico_GetById")]
    [ProducesResponseType(typeof(Result<TecnicoDetalleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<TecnicoDetalleDto>>> GetByIdAsync(
        [FromServices] ILogger<TecnicoController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int id)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Tecnico.GetById");

        var tecnico = await _service.GetByIdAsync(id);

        return this.FromResult(tecnico);
    }

    /// <summary>Registrar técnicos.</summary>
    /// <param name="logger">Registrador de la operación, resuelto por inyección de dependencias.</param>
    /// <param name="dto">Datos de entrada de la operación.</param>
    /// <response code="201">Recurso creado; Location permite consultar su detalle.</response>
    /// <response code="400">Entrada inválida; consulte los errores por campo.</response>
    /// <response code="415">El tipo de contenido no está admitido.</response>
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

        return CreatedAtRoute(
            "Tecnico_GetById",
            new { id = result.Data!.Id },
            result);
    }

    /// <summary>Actualizar técnicos.</summary>
    /// <param name="logger">Registrador de la operación, resuelto por inyección de dependencias.</param>
    /// <param name="id">Identificador positivo del recurso.</param>
    /// <param name="dto">Datos de entrada de la operación.</param>
    /// <response code="200">Operación completada; el cuerpo contiene Result.</response>
    /// <response code="400">Entrada inválida; consulte los errores por campo.</response>
    /// <response code="404">El recurso solicitado no existe.</response>
    /// <response code="415">El tipo de contenido no está admitido.</response>
    [EndpointSummary("Actualizar técnicos")]
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Result<object?>), StatusCodes.Status200OK)]
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

    /// <summary>Eliminar técnicos.</summary>
    /// <param name="logger">Registrador de la operación, resuelto por inyección de dependencias.</param>
    /// <param name="id">Identificador positivo del recurso.</param>
    /// <response code="200">Operación completada; el cuerpo contiene Result.</response>
    /// <response code="400">Entrada inválida; consulte los errores por campo.</response>
    /// <response code="404">El recurso solicitado no existe.</response>
    /// <response code="409">La operación entra en conflicto con los datos o las reglas de negocio.</response>
    [EndpointSummary("Eliminar técnicos")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(Result<object?>), StatusCodes.Status200OK)]
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

    /// <summary>Listar asociaciones técnico-especialidad.</summary>
    /// <param name="logger">Registrador de la operación, resuelto por inyección de dependencias.</param>
    /// <param name="tecnicoId">Identificador positivo del recurso.</param>
    /// <response code="200">Operación completada; el cuerpo contiene Result.</response>
    /// <response code="400">Entrada inválida; consulte los errores por campo.</response>
    /// <response code="404">El recurso solicitado no existe.</response>
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

    /// <summary>Consultar detalle de asociaciones técnico-especialidad.</summary>
    /// <param name="logger">Registrador de la operación, resuelto por inyección de dependencias.</param>
    /// <param name="tecnicoId">Identificador positivo del recurso.</param>
    /// <param name="especialidadId">Identificador positivo del recurso.</param>
    /// <response code="200">Operación completada; el cuerpo contiene Result.</response>
    /// <response code="400">Entrada inválida; consulte los errores por campo.</response>
    /// <response code="404">El recurso solicitado no existe.</response>
    [EndpointSummary("Consultar detalle de asociaciones técnico-especialidad")]
    [HttpGet("{tecnicoId:int}/especialidades/{especialidadId:int}", Name = "Tecnico_GetEspecialidad")]
    [ProducesResponseType(typeof(Result<TecnicoEspecialidadDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result<TecnicoEspecialidadDto>>> ConsultarEspecialidadAsync(
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

    /// <summary>Registrar asociaciones técnico-especialidad.</summary>
    /// <param name="logger">Registrador de la operación, resuelto por inyección de dependencias.</param>
    /// <param name="tecnicoId">Identificador positivo del recurso.</param>
    /// <param name="dto">Datos de entrada de la operación.</param>
    /// <response code="201">Recurso creado; Location permite consultar su detalle.</response>
    /// <response code="400">Entrada inválida; consulte los errores por campo.</response>
    /// <response code="404">El recurso solicitado no existe.</response>
    /// <response code="409">La operación entra en conflicto con los datos o las reglas de negocio.</response>
    /// <response code="415">El tipo de contenido no está admitido.</response>
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

        return CreatedAtRoute(
            "Tecnico_GetEspecialidad",
            new
            {
                tecnicoId = tecnicoEspecialidad!.TecnicoId,
                especialidadId = tecnicoEspecialidad!.EspecialidadId
            },
            result);
    }

    /// <summary>Actualizar asociaciones técnico-especialidad.</summary>
    /// <param name="logger">Registrador de la operación, resuelto por inyección de dependencias.</param>
    /// <param name="tecnicoId">Identificador positivo del recurso.</param>
    /// <param name="especialidadId">Identificador positivo del recurso.</param>
    /// <param name="dto">Datos de entrada de la operación.</param>
    /// <response code="200">Operación completada; el cuerpo contiene Result.</response>
    /// <response code="400">Entrada inválida; consulte los errores por campo.</response>
    /// <response code="404">El recurso solicitado no existe.</response>
    /// <response code="415">El tipo de contenido no está admitido.</response>
    [EndpointSummary("Actualizar asociaciones técnico-especialidad")]
    [HttpPut("{tecnicoId:int}/especialidades/{especialidadId:int}")]
    [ProducesResponseType(typeof(Result<object?>), StatusCodes.Status200OK)]
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

    /// <summary>Eliminar asociaciones técnico-especialidad.</summary>
    /// <param name="logger">Registrador de la operación, resuelto por inyección de dependencias.</param>
    /// <param name="tecnicoId">Identificador positivo del recurso.</param>
    /// <param name="especialidadId">Identificador positivo del recurso.</param>
    /// <response code="200">Operación completada; el cuerpo contiene Result.</response>
    /// <response code="400">Entrada inválida; consulte los errores por campo.</response>
    /// <response code="404">El recurso solicitado no existe.</response>
    [EndpointSummary("Eliminar asociaciones técnico-especialidad")]
    [HttpDelete("{tecnicoId:int}/especialidades/{especialidadId:int}")]
    [ProducesResponseType(typeof(Result<object?>), StatusCodes.Status200OK)]
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
