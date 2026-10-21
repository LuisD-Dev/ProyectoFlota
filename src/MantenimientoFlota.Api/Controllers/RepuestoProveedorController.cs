using System.ComponentModel.DataAnnotations;
using MantenimientoFlota.Api.Contracts;

using MantenimientoFlota.Application.DTOs.RepuestoProveedor;
using MantenimientoFlota.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace MantenimientoFlota.Api.Controllers;

[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
[ApiController]
[Route("api/[controller]")]
public sealed class RepuestoProveedorController : ControllerBase
{
    private readonly IRepuestoProveedorService _service;

    public RepuestoProveedorController(IRepuestoProveedorService service)
    {
        _service = service;
    }

    [EndpointSummary("Listar asociaciones repuesto-proveedor")]
    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<RepuestoProveedorDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<RepuestoProveedorDto>>> GetAllAsync(
        [FromServices] ILogger<RepuestoProveedorController> logger)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "RepuestoProveedor.GetAllAsync");
        var repuestoProveedores = await _service.GetAllAsync();

        return Ok(repuestoProveedores);
    }

    [EndpointSummary("Consultar detalle de asociaciones repuesto-proveedor")]
    [HttpGet("{repuestoId:int}/{proveedorId:int}")]
    [ProducesResponseType(
        typeof(RepuestoProveedorDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RepuestoProveedorDto>> GetById(
        [FromServices] ILogger<RepuestoProveedorController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int repuestoId,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int proveedorId)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "RepuestoProveedor.GetById");

        var repuestoProveedor = await _service.GetByIdAsync(
            repuestoId,
            proveedorId);

        return Ok(repuestoProveedor);
    }

    [EndpointSummary("Registrar asociaciones repuesto-proveedor")]
    [HttpPost]
    [ProducesResponseType(
        typeof(RepuestoProveedorDto),
        StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public async Task<ActionResult<RepuestoProveedorDto>> CreateAsync(
        [FromServices] ILogger<RepuestoProveedorController> logger,
        CrearRepuestoProveedorDto dto)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "RepuestoProveedor.CreateAsync");
        var result = await _service.CreateAsync(dto);

        var repuestoProveedor = result;

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                repuestoId = repuestoProveedor.RepuestoId,
                proveedorId = repuestoProveedor.ProveedorId
            },
            repuestoProveedor);
    }

    [EndpointSummary("Actualizar asociaciones repuesto-proveedor")]
    [HttpPut("{repuestoId:int}/{proveedorId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public async Task<IActionResult> UpdateAsync(
        [FromServices] ILogger<RepuestoProveedorController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int repuestoId,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int proveedorId,
        ActualizarRepuestoProveedorDto dto)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "RepuestoProveedor.UpdateAsync");

        await _service.UpdateAsync(
            repuestoId,
            proveedorId,
            dto);

        return NoContent();
    }

    [EndpointSummary("Eliminar asociaciones repuesto-proveedor")]
    [HttpDelete("{repuestoId:int}/{proveedorId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(
        [FromServices] ILogger<RepuestoProveedorController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int repuestoId,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int proveedorId)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "RepuestoProveedor.DeleteAsync");

        await _service.DeleteAsync(repuestoId, proveedorId);

        return NoContent();
    }

}
