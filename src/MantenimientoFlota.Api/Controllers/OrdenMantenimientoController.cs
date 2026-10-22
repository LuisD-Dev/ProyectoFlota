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

    [EndpointSummary("Listar órdenes de mantenimiento")]
    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<OrdenMantenimientoDto>),
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

    [EndpointSummary("Consultar detalle de órdenes de mantenimiento")]
    [HttpGet("{id:int}")]
    [ProducesResponseType(
        typeof(Result<OrdenMantenimientoDetalleDto>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<OrdenMantenimientoDetalleDto>>> GetById(
        [FromServices] ILogger<OrdenMantenimientoController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int id)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "OrdenMantenimiento.GetById");

        var orden = await _service.GetByIdAsync(id);

        return this.FromResult(orden);
    }

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

        return CreatedAtAction(
            nameof(GetById),
            new { id = orden!.Id },
            result);
    }
}
