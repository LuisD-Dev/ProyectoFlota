using MantenimientoFlota.Application.Common.Results;
using MantenimientoFlota.Application.DTOs.Proveedor;
using MantenimientoFlota.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MantenimientoFlota.Api.Controllers;

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
    [ProducesResponseType(typeof(ProveedorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProveedorDto>> GetById(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                mensaje = "El identificador debe ser mayor que cero."
            });
        }

        var proveedor = await _service.GetByIdAsync(id);

        if (proveedor is null)
        {
            return NotFound(new
            {
                mensaje = $"No se encontró el proveedor con id {id}."
            });
        }

        return Ok(proveedor);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProveedorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProveedorDto>> CreateAsync(
        CrearProveedorDto dto)
    {
        var result = await _service.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value!.Id },
            result.Value);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync(
        int id,
        ActualizarProveedorDto dto)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                mensaje = "El identificador debe ser mayor que cero."
            });
        }

        var result = await _service.UpdateAsync(id, dto);

        if (result.Status == ProveedorOperationStatus.DoesNotExist)
        {
            return NotFound(new
            {
                mensaje = $"No se encontró el proveedor con id {id}."
            });
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                mensaje = "El identificador debe ser mayor que cero."
            });
        }

        var result = await _service.DeleteAsync(id);

        if (result.Status == ProveedorOperationStatus.DoesNotExist)
        {
            return NotFound(new
            {
                mensaje = $"No se encontró el proveedor con id {id}."
            });
        }

        if (result.Status == ProveedorOperationStatus.HasAssociatedParts)
        {
            return Conflict(new
            {
                mensaje = "No se puede eliminar el proveedor porque tiene repuestos asociados."
            });
        }

        return NoContent();
    }
}
