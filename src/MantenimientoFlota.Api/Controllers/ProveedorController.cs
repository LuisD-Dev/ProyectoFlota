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

    [EndpointSummary("Listar proveedores")]
    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<ProveedorDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<IReadOnlyList<ProveedorDto>>>> GetAllAsync(
        [FromServices] ILogger<ProveedorController> logger)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Proveedor.GetAllAsync");
        var proveedores = await _service.GetAllAsync();

        return this.FromResult(proveedores);
    }

    [EndpointSummary("Consultar detalle de proveedores")]
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Result<ProveedorDetalleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<ProveedorDetalleDto>>> GetById(
        [FromServices] ILogger<ProveedorController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int id)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Proveedor.GetById");

        var proveedor = await _service.GetByIdAsync(id);

        return this.FromResult(proveedor);
    }

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

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Data!.Id },
            result);
    }

    [EndpointSummary("Actualizar proveedores")]
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
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

    [EndpointSummary("Eliminar proveedores")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
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
