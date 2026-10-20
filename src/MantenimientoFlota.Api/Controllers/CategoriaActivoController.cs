using MantenimientoFlota.Api.Contracts;
using MantenimientoFlota.Application.Common.Exceptions;
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
    [ProducesResponseType(typeof(CategoriaActivoDetalleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoriaActivoDetalleDto>> GetById(int id)
    {
        if (id <= 0)
        {
            throw new InvalidRequestException("El identificador debe ser mayor que cero.");
        }

        var categoria = await _service.GetByIdAsync(id);

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

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
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
