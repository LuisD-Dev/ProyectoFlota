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

    /// <summary>Listar categorías de activo.</summary>
    /// <param name="logger">Registrador de la operación, resuelto por inyección de dependencias.</param>
    /// <response code="200">Operación completada; el cuerpo contiene Result.</response>
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

    /// <summary>Consultar detalle de categorías de activo.</summary>
    /// <param name="logger">Registrador de la operación, resuelto por inyección de dependencias.</param>
    /// <param name="id">Identificador positivo del recurso.</param>
    /// <response code="200">Operación completada; el cuerpo contiene Result.</response>
    /// <response code="400">Entrada inválida; consulte los errores por campo.</response>
    /// <response code="404">El recurso solicitado no existe.</response>
    [EndpointSummary("Consultar detalle de categorías de activo")]
    [HttpGet("{id:int}", Name = "CategoriaActivo_GetById")]
    [ProducesResponseType(typeof(Result<CategoriaActivoDetalleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<CategoriaActivoDetalleDto>>> GetByIdAsync(
        [FromServices] ILogger<CategoriaActivoController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int id)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "CategoriaActivo.GetById");

        var categoria = await _service.GetByIdAsync(id);

        return this.FromResult(categoria);
    }

    /// <summary>Registrar categorías de activo.</summary>
    /// <param name="logger">Registrador de la operación, resuelto por inyección de dependencias.</param>
    /// <param name="dto">Datos de entrada de la operación.</param>
    /// <response code="201">Recurso creado; Location permite consultar su detalle.</response>
    /// <response code="400">Entrada inválida; consulte los errores por campo.</response>
    /// <response code="409">La operación entra en conflicto con los datos o las reglas de negocio.</response>
    /// <response code="415">El tipo de contenido no está admitido.</response>
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

        return CreatedAtRoute(
            "CategoriaActivo_GetById",
            new { id = result.Data!.Id },
            result);
    }

    /// <summary>Actualizar categorías de activo.</summary>
    /// <param name="logger">Registrador de la operación, resuelto por inyección de dependencias.</param>
    /// <param name="id">Identificador positivo del recurso.</param>
    /// <param name="dto">Datos de entrada de la operación.</param>
    /// <response code="200">Operación completada; el cuerpo contiene Result.</response>
    /// <response code="400">Entrada inválida; consulte los errores por campo.</response>
    /// <response code="404">El recurso solicitado no existe.</response>
    /// <response code="409">La operación entra en conflicto con los datos o las reglas de negocio.</response>
    /// <response code="415">El tipo de contenido no está admitido.</response>
    [EndpointSummary("Actualizar categorías de activo")]
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Result<object?>), StatusCodes.Status200OK)]
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

    /// <summary>Eliminar categorías de activo.</summary>
    /// <param name="logger">Registrador de la operación, resuelto por inyección de dependencias.</param>
    /// <param name="id">Identificador positivo del recurso.</param>
    /// <response code="200">Operación completada; el cuerpo contiene Result.</response>
    /// <response code="400">Entrada inválida; consulte los errores por campo.</response>
    /// <response code="404">El recurso solicitado no existe.</response>
    /// <response code="409">La operación entra en conflicto con los datos o las reglas de negocio.</response>
    [EndpointSummary("Eliminar categorías de activo")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(Result<object?>), StatusCodes.Status200OK)]
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
