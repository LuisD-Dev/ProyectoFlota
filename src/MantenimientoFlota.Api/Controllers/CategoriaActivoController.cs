using MantenimientoFlota.Application.Common.Results;
using MantenimientoFlota.Application.DTOs.CategoriaActivo;
using MantenimientoFlota.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MantenimientoFlota.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CategoriaActivoController : ControllerBase
{
    private readonly ICategoriaActivoService _service;

    public CategoriaActivoController(ICategoriaActivoService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IEnumerable<CategoriaActivoDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CategoriaActivoDto>>> GetAllAsync()
    {
        var categorias = await _service.GetAllAsync();

        return Ok(categorias);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CategoriaActivoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoriaActivoDto>> GetById(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                mensaje = "El identificador debe ser mayor que cero."
            });
        }

        var categoria = await _service.GetByIdAsync(id);

        if (categoria is null)
        {
            return NotFound(new
            {
                mensaje = $"No se encontró la categoría de activo con id {id}."
            });
        }

        return Ok(categoria);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CategoriaActivoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CategoriaActivoDto>> CreateAsync(
        CrearCategoriaActivoDto dto)
    {
        var result = await _service.CreateAsync(dto);

        if (result.Status == CategoriaActivoOperationStatus.NameAlreadyExists)
        {
            return Conflict(new
            {
                mensaje = "Ya existe una categoría de activo con el nombre indicado."
            });
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value!.Id },
            result.Value);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateAsync(
        int id,
        ActualizarCategoriaActivoDto dto)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                mensaje = "El identificador debe ser mayor que cero."
            });
        }

        var result = await _service.UpdateAsync(id, dto);

        if (result.Status == CategoriaActivoOperationStatus.DoesNotExist)
        {
            return NotFound(new
            {
                mensaje = $"No se encontró la categoría de activo con id {id}."
            });
        }

        if (result.Status == CategoriaActivoOperationStatus.NameAlreadyExists)
        {
            return Conflict(new
            {
                mensaje = "Ya existe una categoría de activo con el nombre indicado."
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

        if (result.Status == CategoriaActivoOperationStatus.DoesNotExist)
        {
            return NotFound(new
            {
                mensaje = $"No se encontró la categoría de activo con id {id}."
            });
        }

        if (result.Status == CategoriaActivoOperationStatus.HasAssociatedAssets)
        {
            return Conflict(new
            {
                mensaje = "No se puede eliminar la categoría porque tiene activos asociados."
            });
        }

        return NoContent();
    }
}
