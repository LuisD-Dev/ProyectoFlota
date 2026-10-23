using MantenimientoFlota.Domain.DTO.PatternResult;
using MantenimientoFlota.Api.Common;
using System.ComponentModel.DataAnnotations;
using MantenimientoFlota.Api.Contracts;

using MantenimientoFlota.Domain.DTO.Proveedor;
using MantenimientoFlota.Application.Interfaces;
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

    /// <summary>Listar proveedores.</summary>
    /// <param name="logger">Registrador de la operación, resuelto por inyección de dependencias.</param>
    /// <response code="200">Operación completada; el cuerpo contiene Result.</response>
    [EndpointSummary("Listar proveedores")]
    [HttpGet]
    [ProducesResponseType(
        typeof(Result<IReadOnlyList<ProveedorDto>>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<IReadOnlyList<ProveedorDto>>>> GetAllAsync(
        [FromServices] ILogger<ProveedorController> logger)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Proveedor.GetAllAsync");
        var proveedores = await _service.GetAllAsync();

        return this.FromResult(proveedores);
    }

    /// <summary>Consultar detalle de proveedores.</summary>
    /// <param name="logger">Registrador de la operación, resuelto por inyección de dependencias.</param>
    /// <param name="id">Identificador positivo del recurso.</param>
    /// <response code="200">Operación completada; el cuerpo contiene Result.</response>
    /// <response code="400">Entrada inválida; consulte los errores por campo.</response>
    /// <response code="404">El recurso solicitado no existe.</response>
    [EndpointSummary("Consultar detalle de proveedores")]
    [HttpGet("{id:int}", Name = "Proveedor_GetById")]
    [ProducesResponseType(typeof(Result<ProveedorDetalleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<ProveedorDetalleDto>>> GetByIdAsync(
        [FromServices] ILogger<ProveedorController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int id)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Proveedor.GetById");

        var proveedor = await _service.GetByIdAsync(id);

        return this.FromResult(proveedor);
    }

    /// <summary>Registrar proveedores.</summary>
    /// <param name="logger">Registrador de la operación, resuelto por inyección de dependencias.</param>
    /// <param name="dto">Datos de entrada de la operación.</param>
    /// <response code="201">Recurso creado; Location permite consultar su detalle.</response>
    /// <response code="400">Entrada inválida; consulte los errores por campo.</response>
    /// <response code="415">El tipo de contenido no está admitido.</response>
    [EndpointSummary("Registrar proveedores")]
    [HttpPost]
    [ProducesResponseType(typeof(Result<ProveedorDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public async Task<ActionResult<Result<ProveedorDto>>> CreateAsync(
        [FromServices] ILogger<ProveedorController> logger,
        CrearProveedorDto dto)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Proveedor.CreateAsync");
        var result = await _service.CreateAsync(dto);

        if (!result.IsSuccess) return this.FromResult(result);

        return CreatedAtRoute(
            "Proveedor_GetById",
            new { id = result.Data!.Id },
            result);
    }

    /// <summary>Actualizar proveedores.</summary>
    /// <param name="logger">Registrador de la operación, resuelto por inyección de dependencias.</param>
    /// <param name="id">Identificador positivo del recurso.</param>
    /// <param name="dto">Datos de entrada de la operación.</param>
    /// <response code="200">Operación completada; el cuerpo contiene Result.</response>
    /// <response code="400">Entrada inválida; consulte los errores por campo.</response>
    /// <response code="404">El recurso solicitado no existe.</response>
    /// <response code="415">El tipo de contenido no está admitido.</response>
    [EndpointSummary("Actualizar proveedores")]
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Result<object?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public async Task<IActionResult> UpdateAsync(
        [FromServices] ILogger<ProveedorController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int id,
        ActualizarProveedorDto dto)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Proveedor.UpdateAsync");

        var result = await _service.UpdateAsync(id, dto);

        return this.FromResult(result);
    }

    /// <summary>Eliminar proveedores.</summary>
    /// <param name="logger">Registrador de la operación, resuelto por inyección de dependencias.</param>
    /// <param name="id">Identificador positivo del recurso.</param>
    /// <response code="200">Operación completada; el cuerpo contiene Result.</response>
    /// <response code="400">Entrada inválida; consulte los errores por campo.</response>
    /// <response code="404">El recurso solicitado no existe.</response>
    /// <response code="409">La operación entra en conflicto con los datos o las reglas de negocio.</response>
    [EndpointSummary("Eliminar proveedores")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(Result<object?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteAsync(
        [FromServices] ILogger<ProveedorController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int id)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Proveedor.DeleteAsync");

        var result = await _service.DeleteAsync(id);

        return this.FromResult(result);
    }
}
