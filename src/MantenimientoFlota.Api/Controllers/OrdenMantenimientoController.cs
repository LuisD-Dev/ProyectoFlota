using MantenimientoFlota.Domain.DTO.PatternResult;
using MantenimientoFlota.Api.Common;
using System.ComponentModel.DataAnnotations;
using MantenimientoFlota.Api.Contracts;

using MantenimientoFlota.Domain.DTO.OrdenMantenimiento;
using MantenimientoFlota.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MantenimientoFlota.Api.Controllers;

[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
[ApiController]
[Route("api/[controller]")]
public sealed class OrdenMantenimientoController : ControllerBase
{
    private readonly IOrdenMantenimientoService _service;

    public OrdenMantenimientoController(IOrdenMantenimientoService service)
    {
        _service = service;
    }

    /// <summary>Listar órdenes de mantenimiento.</summary>
    /// <param name="logger">Registrador de la operación, resuelto por inyección de dependencias.</param>
    /// <param name="activoId">Identificador positivo del recurso.</param>
    /// <response code="200">Operación completada; el cuerpo contiene Result.</response>
    /// <response code="400">Entrada inválida; consulte los errores por campo.</response>
    [EndpointSummary("Listar órdenes de mantenimiento")]
    [HttpGet]
    [ProducesResponseType(
        typeof(Result<IReadOnlyList<OrdenMantenimientoDto>>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result<IReadOnlyList<OrdenMantenimientoDto>>>> GetAllAsync(
        [FromServices] ILogger<OrdenMantenimientoController> logger,
        [FromQuery] [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int? activoId = null)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "OrdenMantenimiento.GetAllAsync");

        var ordenes = await _service.GetAllAsync(activoId);

        return this.FromResult(ordenes);
    }

    /// <summary>Consultar detalle de órdenes de mantenimiento.</summary>
    /// <param name="logger">Registrador de la operación, resuelto por inyección de dependencias.</param>
    /// <param name="id">Identificador positivo del recurso.</param>
    /// <response code="200">Operación completada; el cuerpo contiene Result.</response>
    /// <response code="400">Entrada inválida; consulte los errores por campo.</response>
    /// <response code="404">El recurso solicitado no existe.</response>
    [EndpointSummary("Consultar detalle de órdenes de mantenimiento")]
    [HttpGet("{id:int}", Name = "OrdenMantenimiento_GetById")]
    [ProducesResponseType(
        typeof(Result<OrdenMantenimientoDetalleDto>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<OrdenMantenimientoDetalleDto>>> GetByIdAsync(
        [FromServices] ILogger<OrdenMantenimientoController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int id)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "OrdenMantenimiento.GetById");

        var orden = await _service.GetByIdAsync(id);

        return this.FromResult(orden);
    }

    /// <summary>Registrar órdenes de mantenimiento.</summary>
    /// <param name="logger">Registrador de la operación, resuelto por inyección de dependencias.</param>
    /// <param name="dto">Datos de entrada de la operación.</param>
    /// <response code="201">Recurso creado; Location permite consultar su detalle.</response>
    /// <response code="400">Entrada inválida; consulte los errores por campo.</response>
    /// <response code="404">El recurso solicitado no existe.</response>
    /// <response code="415">El tipo de contenido no está admitido.</response>
    [EndpointSummary("Registrar órdenes de mantenimiento")]
    [HttpPost]
    [ProducesResponseType(
        typeof(Result<OrdenMantenimientoDto>),
        StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public async Task<ActionResult<Result<OrdenMantenimientoDto>>> CreateAsync(
        [FromServices] ILogger<OrdenMantenimientoController> logger,
        CrearOrdenMantenimientoDto dto)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "OrdenMantenimiento.CreateAsync");
        var result = await _service.CreateAsync(dto);

        var orden = result.Data;

        if (!result.IsSuccess) return this.FromResult(result);

        return CreatedAtRoute(
            "OrdenMantenimiento_GetById",
            new { id = orden!.Id },
            result);
    }
}
