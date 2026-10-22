using MantenimientoFlota.Domain.DTO.RepuestoProveedor;
using MantenimientoFlota.Domain.DTO.PatternResult;
using MantenimientoFlota.Api.Common;
using System.ComponentModel.DataAnnotations;
using MantenimientoFlota.Api.Contracts;

using MantenimientoFlota.Domain.DTO.Repuesto;
using MantenimientoFlota.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MantenimientoFlota.Api.Controllers;

[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
[ApiController]
[Route("api/[controller]")]
public sealed class RepuestoController : ControllerBase
{
    private readonly IRepuestoService _service;
    private readonly IRepuestoProveedorService _relaciones;

    public RepuestoController(IRepuestoService service, IRepuestoProveedorService relaciones)
    {
        _service = service;
        _relaciones = relaciones;
    }

    [EndpointSummary("Listar repuestos")]
    [HttpGet]
    [ProducesResponseType(typeof(Result<IEnumerable<RepuestoDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result<IEnumerable<RepuestoDto>>>> GetAllAsync(
        [FromServices] ILogger<RepuestoController> logger,
        [FromQuery] bool stockBajo = false)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Repuesto.GetAllAsync");
        var repuestos = await _service.GetAllAsync(stockBajo);

        return this.FromResult(repuestos);
    }

    [EndpointSummary("Consultar detalle de repuestos")]
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Result<RepuestoDetalleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<RepuestoDetalleDto>>> GetById(
        [FromServices] ILogger<RepuestoController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int id)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Repuesto.GetById");

        var repuesto = await _service.GetByIdAsync(id);

        return this.FromResult(repuesto);
    }

    [EndpointSummary("Registrar repuestos")]
    [HttpPost]
    [ProducesResponseType(typeof(Result<RepuestoDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public async Task<ActionResult<Result<RepuestoDto>>> CreateAsync(
        [FromServices] ILogger<RepuestoController> logger,
        CrearRepuestoDto dto)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Repuesto.CreateAsync");
        var result = await _service.CreateAsync(dto);

        if (!result.IsSuccess) return this.FromResult(result);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Data!.Id },
            result);
    }

    [EndpointSummary("Actualizar repuestos")]
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public async Task<IActionResult> UpdateAsync(
        [FromServices] ILogger<RepuestoController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int id, ActualizarRepuestoDto dto)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Repuesto.UpdateAsync");

        var result = await _service.UpdateAsync(id, dto);

        return this.FromResult(result);
    }

    [EndpointSummary("Eliminar repuestos")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteAsync(
        [FromServices] ILogger<RepuestoController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int id)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Repuesto.DeleteAsync");

        var result = await _service.DeleteAsync(id);

        return this.FromResult(result);
    }

    [EndpointSummary("Listar asociaciones repuesto-proveedor")]
    [HttpGet("{repuestoId:int}/proveedores")]
    [ProducesResponseType(
        typeof(Result<IReadOnlyList<RepuestoProveedorDto>>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<IReadOnlyList<RepuestoProveedorDto>>>> ListarProveedoresAsync(
        [FromServices] ILogger<RepuestoController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int repuestoId)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "RepuestoProveedor.ListarProveedoresAsync");
        var repuestoProveedores = await _relaciones.GetAllAsync(repuestoId);

        return this.FromResult(repuestoProveedores);
    }

    [EndpointSummary("Consultar detalle de asociaciones repuesto-proveedor")]
    [HttpGet("{repuestoId:int}/proveedores/{proveedorId:int}")]
    [ProducesResponseType(
        typeof(Result<RepuestoProveedorDto>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<RepuestoProveedorDto>>> ConsultarProveedor(
        [FromServices] ILogger<RepuestoController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int repuestoId,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int proveedorId)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "RepuestoProveedor.ConsultarProveedor");

        var repuestoProveedor = await _relaciones.GetByIdAsync(
            repuestoId,
            proveedorId);

        return this.FromResult(repuestoProveedor);
    }

    [EndpointSummary("Registrar asociaciones repuesto-proveedor")]
    [HttpPost("{repuestoId:int}/proveedores")]
    [ProducesResponseType(
        typeof(Result<RepuestoProveedorDto>),
        StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public async Task<ActionResult<Result<RepuestoProveedorDto>>> AsociarProveedorAsync(
        [FromServices] ILogger<RepuestoController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int repuestoId,
        CrearRepuestoProveedorDto dto)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "RepuestoProveedor.AsociarProveedorAsync");
        var result = await _relaciones.CreateAsync(repuestoId, dto);

        var repuestoProveedor = result.Data;

        if (!result.IsSuccess) return this.FromResult(result);

        return CreatedAtAction(
            nameof(ConsultarProveedor),
            new
            {
                repuestoId = repuestoProveedor!.RepuestoId,
                proveedorId = repuestoProveedor!.ProveedorId
            },
            result);
    }

    [EndpointSummary("Actualizar asociaciones repuesto-proveedor")]
    [HttpPut("{repuestoId:int}/proveedores/{proveedorId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public async Task<IActionResult> ActualizarProveedorAsync(
        [FromServices] ILogger<RepuestoController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int repuestoId,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int proveedorId,
        ActualizarRepuestoProveedorDto dto)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "RepuestoProveedor.ActualizarProveedorAsync");

        var result = await _relaciones.UpdateAsync(
            repuestoId,
            proveedorId,
            dto);

        return this.FromResult(result);
    }

    [EndpointSummary("Eliminar asociaciones repuesto-proveedor")]
    [HttpDelete("{repuestoId:int}/proveedores/{proveedorId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DesasociarProveedorAsync(
        [FromServices] ILogger<RepuestoController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int repuestoId,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int proveedorId)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "RepuestoProveedor.DesasociarProveedorAsync");

        var result = await _relaciones.DeleteAsync(repuestoId, proveedorId);

        return this.FromResult(result);
    }

}
