using MantenimientoFlota.Application.Common.Results;
using MantenimientoFlota.Application.DTOs.OrdenMantenimiento;
using MantenimientoFlota.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MantenimientoFlota.Api.Controllers;

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
    public async Task<ActionResult<IReadOnlyList<OrdenMantenimientoDto>>> GetAllAsync()
    {
        var ordenes = await _service.GetAllAsync();

        return Ok(ordenes);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(
        typeof(OrdenMantenimientoDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrdenMantenimientoDto>> GetById(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                mensaje = "El identificador debe ser mayor que cero."
            });
        }

        var orden = await _service.GetByIdAsync(id);

        if (orden is null)
        {
            return NotFound(new
            {
                mensaje = $"No se encontró la orden de mantenimiento con id {id}."
            });
        }

        return Ok(orden);
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(OrdenMantenimientoDto),
        StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrdenMantenimientoDto>> CreateAsync(
        CrearOrdenMantenimientoDto dto)
    {
        var result = await _service.CreateAsync(dto);

        if (result.Status ==
            OrdenMantenimientoOperationStatus.ActivoDoesNotExist)
        {
            return NotFound(new
            {
                mensaje = $"No se encontró el activo con id {dto.ActivoId}."
            });
        }

        var orden = result.Value!;

        return CreatedAtAction(
            nameof(GetById),
            new { id = orden.Id },
            orden);
    }
}
