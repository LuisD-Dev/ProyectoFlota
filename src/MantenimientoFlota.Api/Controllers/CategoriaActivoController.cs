using MantenimientoFlota.Domain.DTO.PatternResult;
using MantenimientoFlota.Api.Common;
using System.ComponentModel.DataAnnotations;
using MantenimientoFlota.Api.Contracts;

using MantenimientoFlota.Domain.DTO.CategoriaActivo;
using MantenimientoFlota.Application.Interfaces;
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
        typeof(Result<IEnumerable<CategoriaActivoDto>>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<IEnumerable<CategoriaActivoDto>>>> GetAllAsync(
        [FromServices] ILogger<CategoriaActivoController> logger)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "CategoriaActivo.GetAllAsync");
        var categorias = await _service.GetAllAsync();

        return this.FromResult(categorias);
    }

    [EndpointSummary("Consultar detalle de categorías de activo")]
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Result<CategoriaActivoDetalleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<CategoriaActivoDetalleDto>>> GetById(
        [FromServices] ILogger<CategoriaActivoController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int id)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "CategoriaActivo.GetById");

        var categoria = await _service.GetByIdAsync(id);

        return this.FromResult(categoria);
    }

    [EndpointSummary("Registrar categorías de activo")]
    [HttpPost]
    [ProducesResponseType(typeof(Result<CategoriaActivoDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public async Task<ActionResult<Result<CategoriaActivoDto>>> CreateAsync(
        [FromServices] ILogger<CategoriaActivoController> logger,
        CrearCategoriaActivoDto dto)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "CategoriaActivo.CreateAsync");
        var result = await _service.CreateAsync(dto);

        if (!result.IsSuccess) return this.FromResult(result);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Data!.Id },
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

        var result = await _service.UpdateAsync(id, dto);

        return this.FromResult(result);
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

        var result = await _service.DeleteAsync(id);

        return this.FromResult(result);
    }
}
