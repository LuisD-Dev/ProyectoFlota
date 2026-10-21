using System.ComponentModel.DataAnnotations;
using MantenimientoFlota.Api.Contracts;

using MantenimientoFlota.Application.DTOs.OrdenMantenimiento;
using MantenimientoFlota.Application.Interfaces.Services;
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
    public async Task<ActionResult<IReadOnlyList<OrdenMantenimientoDto>>> GetAllAsync(
        [FromServices] ILogger<OrdenMantenimientoController> logger,
        [FromQuery] [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int? activoId = null)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "OrdenMantenimiento.GetAllAsync");

        var ordenes = await _service.GetAllAsync(activoId);

        return Ok(ordenes);
    }

    [EndpointSummary("Consultar detalle de órdenes de mantenimiento")]
    [HttpGet("{id:int}")]
    [ProducesResponseType(
        typeof(OrdenMantenimientoDetalleDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrdenMantenimientoDetalleDto>> GetById(
        [FromServices] ILogger<OrdenMantenimientoController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int id)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "OrdenMantenimiento.GetById");

        var orden = await _service.GetByIdAsync(id);

        return Ok(orden);
    }

    [EndpointSummary("Registrar órdenes de mantenimiento")]
    [HttpPost]
    [ProducesResponseType(
        typeof(OrdenMantenimientoDto),
        StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public async Task<ActionResult<OrdenMantenimientoDto>> CreateAsync(
        [FromServices] ILogger<OrdenMantenimientoController> logger,
        CrearOrdenMantenimientoDto dto)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "OrdenMantenimiento.CreateAsync");
        var result = await _service.CreateAsync(dto);

        var orden = result;

        return CreatedAtAction(
            nameof(GetById),
            new { id = orden.Id },
            orden);
    }
}
