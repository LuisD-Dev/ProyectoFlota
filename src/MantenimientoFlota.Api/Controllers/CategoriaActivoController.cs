using System.ComponentModel.DataAnnotations;
using MantenimientoFlota.Api.Contracts;

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

    [EndpointSummary("Listar categorías de activo")]
    [HttpGet]
    [ProducesResponseType(
        typeof(IEnumerable<CategoriaActivoDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CategoriaActivoDto>>> GetAllAsync(
        [FromServices] ILogger<CategoriaActivoController> logger)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "CategoriaActivo.GetAllAsync");
        var categorias = await _service.GetAllAsync();

        return Ok(categorias);
    }

    [EndpointSummary("Consultar detalle de categorías de activo")]
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CategoriaActivoDetalleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoriaActivoDetalleDto>> GetById(
        [FromServices] ILogger<CategoriaActivoController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int id)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "CategoriaActivo.GetById");

        var categoria = await _service.GetByIdAsync(id);

        return Ok(categoria);
    }

    [EndpointSummary("Registrar categorías de activo")]
    [HttpPost]
    [ProducesResponseType(typeof(CategoriaActivoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public async Task<ActionResult<CategoriaActivoDto>> CreateAsync(
        [FromServices] ILogger<CategoriaActivoController> logger,
        CrearCategoriaActivoDto dto)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "CategoriaActivo.CreateAsync");
        var result = await _service.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
    }

    [EndpointSummary("Actualizar categorías de activo")]
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public async Task<IActionResult> UpdateAsync(
        [FromServices] ILogger<CategoriaActivoController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int id,
        ActualizarCategoriaActivoDto dto)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "CategoriaActivo.UpdateAsync");

        await _service.UpdateAsync(id, dto);

        return NoContent();
    }

    [EndpointSummary("Eliminar categorías de activo")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteAsync(
        [FromServices] ILogger<CategoriaActivoController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int id)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "CategoriaActivo.DeleteAsync");

        await _service.DeleteAsync(id);

        return NoContent();
    }
}
