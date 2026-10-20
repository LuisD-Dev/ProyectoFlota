using MantenimientoFlota.Api.Contracts;
using MantenimientoFlota.Application.Common.Exceptions;
using MantenimientoFlota.Application.DTOs.RepuestoProveedor;
using MantenimientoFlota.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace MantenimientoFlota.Api.Controllers;

[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
[ApiController]
[Route("api/[controller]")]
public sealed class RepuestoProveedorController : ControllerBase
{
    private readonly IRepuestoProveedorService _service;

    public RepuestoProveedorController(IRepuestoProveedorService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<RepuestoProveedorDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<RepuestoProveedorDto>>> GetAllAsync()
    {
        var repuestoProveedores = await _service.GetAllAsync();

        return Ok(repuestoProveedores);
    }

    [HttpGet("{repuestoId:int}/{proveedorId:int}")]
    [ProducesResponseType(
        typeof(RepuestoProveedorDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RepuestoProveedorDto>> GetById(
        int repuestoId,
        int proveedorId)
    {
        if (!AreValidIds(repuestoId, proveedorId))
        {
            throw new InvalidRequestException("Los identificadores de repuesto y proveedor deben ser mayores que cero.");
        }

        var repuestoProveedor = await _service.GetByIdAsync(
            repuestoId,
            proveedorId);

        return Ok(repuestoProveedor);
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(RepuestoProveedorDto),
        StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RepuestoProveedorDto>> CreateAsync(
        CrearRepuestoProveedorDto dto)
    {
        var result = await _service.CreateAsync(dto);

        var repuestoProveedor = result;

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                repuestoId = repuestoProveedor.RepuestoId,
                proveedorId = repuestoProveedor.ProveedorId
            },
            repuestoProveedor);
    }

    [HttpPut("{repuestoId:int}/{proveedorId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync(
        int repuestoId,
        int proveedorId,
        ActualizarRepuestoProveedorDto dto)
    {
        if (!AreValidIds(repuestoId, proveedorId))
        {
            throw new InvalidRequestException("Los identificadores de repuesto y proveedor deben ser mayores que cero.");
        }

        await _service.UpdateAsync(
            repuestoId,
            proveedorId,
            dto);

        return NoContent();
    }

    [HttpDelete("{repuestoId:int}/{proveedorId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(
        int repuestoId,
        int proveedorId)
    {
        if (!AreValidIds(repuestoId, proveedorId))
        {
            throw new InvalidRequestException("Los identificadores de repuesto y proveedor deben ser mayores que cero.");
        }

        await _service.DeleteAsync(repuestoId, proveedorId);

        return NoContent();
    }

    private static bool AreValidIds(int repuestoId, int proveedorId)
    {
        return repuestoId > 0 && proveedorId > 0;
    }

}
