using MantenimientoFlota.Api.Contracts;
using MantenimientoFlota.Application.Common.Exceptions;
using MantenimientoFlota.Application.DTOs.Proveedor;
using MantenimientoFlota.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace MantenimientoFlota.Api.Controllers;

[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
[ApiController]
[Route("api/[controller]")]
public sealed class ProveedorController : ControllerBase
{
    private readonly IProveedorService _service;

    public ProveedorController(IProveedorService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<ProveedorDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProveedorDto>>> GetAllAsync()
    {
        var proveedores = await _service.GetAllAsync();

        return Ok(proveedores);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProveedorDetalleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProveedorDetalleDto>> GetById(int id)
    {
        if (id <= 0)
        {
            throw new InvalidRequestException("El identificador debe ser mayor que cero.");
        }

        var proveedor = await _service.GetByIdAsync(id);

        return Ok(proveedor);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProveedorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProveedorDto>> CreateAsync(
        CrearProveedorDto dto)
    {
        var result = await _service.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync(
        int id,
        ActualizarProveedorDto dto)
    {
        if (id <= 0)
        {
            throw new InvalidRequestException("El identificador debe ser mayor que cero.");
        }

        await _service.UpdateAsync(id, dto);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        if (id <= 0)
        {
            throw new InvalidRequestException("El identificador debe ser mayor que cero.");
        }

        await _service.DeleteAsync(id);

        return NoContent();
    }
}
