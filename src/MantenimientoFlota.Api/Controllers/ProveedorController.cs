using System.ComponentModel.DataAnnotations;
using MantenimientoFlota.Api.Contracts;

using MantenimientoFlota.Application.DTOs.Proveedor;
using MantenimientoFlota.Application.Interfaces.Services;
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
    public async Task<ActionResult<IReadOnlyList<ProveedorDto>>> GetAllAsync(
        [FromServices] ILogger<ProveedorController> logger)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Proveedor.GetAllAsync");
        var proveedores = await _service.GetAllAsync();

        return Ok(proveedores);
    }

    [EndpointSummary("Consultar detalle de proveedores")]
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProveedorDetalleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProveedorDetalleDto>> GetById(
        [FromServices] ILogger<ProveedorController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int id)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Proveedor.GetById");

        var proveedor = await _service.GetByIdAsync(id);

        return Ok(proveedor);
    }

    [EndpointSummary("Registrar proveedores")]
    [HttpPost]
    [ProducesResponseType(typeof(ProveedorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public async Task<ActionResult<ProveedorDto>> CreateAsync(
        [FromServices] ILogger<ProveedorController> logger,
        CrearProveedorDto dto)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Proveedor.CreateAsync");
        var result = await _service.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
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

        await _service.UpdateAsync(id, dto);

        return NoContent();
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

        await _service.DeleteAsync(id);

        return NoContent();
    }
}
