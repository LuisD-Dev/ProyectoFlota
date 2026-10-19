using MantenimientoFlota.Api.Common;
using MantenimientoFlota.Api.Contracts;
using MantenimientoFlota.Application.Common.Results;
using MantenimientoFlota.Application.DTOs.CategoriaActivo;
using MantenimientoFlota.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace MantenimientoFlota.Api.Controllers;

[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
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
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoriaActivoDto>> GetById(int id)
    {
        if (id <= 0)
        {
            return BadRequest(ApiErrorFactory.Create(HttpContext, StatusCodes.Status400BadRequest, "El identificador debe ser mayor que cero."));
        }

        var categoria = await _service.GetByIdAsync(id);

        if (categoria is null)
        {
            return NotFound(ApiErrorFactory.Create(HttpContext, StatusCodes.Status404NotFound, $"No se encontró la categoría de activo con id {id}."));
        }

        return Ok(categoria);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CategoriaActivoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CategoriaActivoDto>> CreateAsync(
        CrearCategoriaActivoDto dto)
    {
        var result = await _service.CreateAsync(dto);

        if (result.Status == CategoriaActivoOperationStatus.NameAlreadyExists)
        {
            return Conflict(ApiErrorFactory.Create(HttpContext, StatusCodes.Status409Conflict, "Ya existe una categoría de activo con el nombre indicado."));
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value!.Id },
            result.Value);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateAsync(
        int id,
        ActualizarCategoriaActivoDto dto)
    {
        if (id <= 0)
        {
            return BadRequest(ApiErrorFactory.Create(HttpContext, StatusCodes.Status400BadRequest, "El identificador debe ser mayor que cero."));
        }

        var result = await _service.UpdateAsync(id, dto);

        if (result.Status == CategoriaActivoOperationStatus.DoesNotExist)
        {
            return NotFound(ApiErrorFactory.Create(HttpContext, StatusCodes.Status404NotFound, $"No se encontró la categoría de activo con id {id}."));
        }

        if (result.Status == CategoriaActivoOperationStatus.NameAlreadyExists)
        {
            return Conflict(ApiErrorFactory.Create(HttpContext, StatusCodes.Status409Conflict, "Ya existe una categoría de activo con el nombre indicado."));
        }

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
            return BadRequest(ApiErrorFactory.Create(HttpContext, StatusCodes.Status400BadRequest, "El identificador debe ser mayor que cero."));
        }

        var result = await _service.DeleteAsync(id);

        if (result.Status == CategoriaActivoOperationStatus.DoesNotExist)
        {
            return NotFound(ApiErrorFactory.Create(HttpContext, StatusCodes.Status404NotFound, $"No se encontró la categoría de activo con id {id}."));
        }

        if (result.Status == CategoriaActivoOperationStatus.HasAssociatedAssets)
        {
            return Conflict(ApiErrorFactory.Create(HttpContext, StatusCodes.Status409Conflict, "No se puede eliminar la categoría porque tiene activos asociados."));
        }

        return NoContent();
    }
}
