using MantenimientoFlota.Api.Contracts;
using MantenimientoFlota.Application.Common.Exceptions;
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

    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<OrdenMantenimientoDto>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<OrdenMantenimientoDto>>> GetAllAsync(
        [FromQuery] int? activoId = null)
    {
        if (activoId is <= 0)
        {
            throw new InvalidRequestException("El identificador del activo debe ser mayor que cero.");
        }

        var ordenes = await _service.GetAllAsync(activoId);

        return Ok(ordenes);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(
        typeof(OrdenMantenimientoDetalleDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrdenMantenimientoDetalleDto>> GetById(int id)
    {
        if (id <= 0)
        {
            throw new InvalidRequestException("El identificador debe ser mayor que cero.");
        }

        var orden = await _service.GetByIdAsync(id);

        return Ok(orden);
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(OrdenMantenimientoDto),
        StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrdenMantenimientoDto>> CreateAsync(
        CrearOrdenMantenimientoDto dto)
    {
        var result = await _service.CreateAsync(dto);

        var orden = result;

        return CreatedAtAction(
            nameof(GetById),
            new { id = orden.Id },
            orden);
    }
}
