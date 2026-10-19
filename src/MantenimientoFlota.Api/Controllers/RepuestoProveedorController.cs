using MantenimientoFlota.Api.Common;
using MantenimientoFlota.Api.Contracts;
using MantenimientoFlota.Application.Common.Results;
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

    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<RepuestoProveedorDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<RepuestoProveedorDto>>> GetAllAsync()
    {
        var repuestoProveedores = await _service.GetAllAsync();

        return Ok(repuestoProveedores);
    }

    [HttpGet("{repuestoId:int}/{proveedorId:int}")]
    [ProducesResponseType(
        typeof(RepuestoProveedorDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RepuestoProveedorDto>> GetById(
        int repuestoId,
        int proveedorId)
    {
        if (!AreValidIds(repuestoId, proveedorId))
        {
            return BadRequest(CreateInvalidIdsResponse());
        }

        var repuestoProveedor = await _service.GetByIdAsync(
            repuestoId,
            proveedorId);

        if (repuestoProveedor is null)
        {
            return NotFound(CreateNotFoundResponse(repuestoId, proveedorId));
        }

        return Ok(repuestoProveedor);
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(RepuestoProveedorDto),
        StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RepuestoProveedorDto>> CreateAsync(
        CrearRepuestoProveedorDto dto)
    {
        var result = await _service.CreateAsync(dto);

        if (result.Status ==
            RepuestoProveedorOperationStatus.RepuestoDoesNotExist)
        {
            return NotFound(ApiErrorFactory.Create(HttpContext, StatusCodes.Status404NotFound, $"No se encontró el repuesto con id {dto.RepuestoId}."));
        }

        if (result.Status ==
            RepuestoProveedorOperationStatus.ProveedorDoesNotExist)
        {
            return NotFound(ApiErrorFactory.Create(HttpContext, StatusCodes.Status404NotFound, $"No se encontró el proveedor con id {dto.ProveedorId}."));
        }

        if (result.Status == RepuestoProveedorOperationStatus.AlreadyExists)
        {
            return Conflict(ApiErrorFactory.Create(HttpContext, StatusCodes.Status409Conflict, "Ya existe una relación entre el repuesto y el proveedor indicados."));
        }

        var repuestoProveedor = result.Value!;

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                repuestoId = repuestoProveedor.RepuestoId,
                proveedorId = repuestoProveedor.ProveedorId
            },
            repuestoProveedor);
    }

    [HttpPut("{repuestoId:int}/{proveedorId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync(
        int repuestoId,
        int proveedorId,
        ActualizarRepuestoProveedorDto dto)
    {
        if (!AreValidIds(repuestoId, proveedorId))
        {
            return BadRequest(CreateInvalidIdsResponse());
        }

        var result = await _service.UpdateAsync(
            repuestoId,
            proveedorId,
            dto);

        if (result.Status == RepuestoProveedorOperationStatus.DoesNotExist)
        {
            return NotFound(CreateNotFoundResponse(repuestoId, proveedorId));
        }

        return NoContent();
    }

    [HttpDelete("{repuestoId:int}/{proveedorId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(
        int repuestoId,
        int proveedorId)
    {
        if (!AreValidIds(repuestoId, proveedorId))
        {
            return BadRequest(CreateInvalidIdsResponse());
        }

        var result = await _service.DeleteAsync(repuestoId, proveedorId);

        if (result.Status == RepuestoProveedorOperationStatus.DoesNotExist)
        {
            return NotFound(CreateNotFoundResponse(repuestoId, proveedorId));
        }

        return NoContent();
    }

    private static bool AreValidIds(int repuestoId, int proveedorId)
    {
        return repuestoId > 0 && proveedorId > 0;
    }

    private ApiErrorResponse CreateInvalidIdsResponse()
    {
        return ApiErrorFactory.Create(HttpContext, StatusCodes.Status400BadRequest, "Los identificadores de repuesto y proveedor deben ser mayores que cero.");
    }

    private ApiErrorResponse CreateNotFoundResponse(
        int repuestoId,
        int proveedorId)
    {
        return ApiErrorFactory.Create(HttpContext, StatusCodes.Status404NotFound, $"No se encontró la relación entre el repuesto con id {repuestoId} y el proveedor con id {proveedorId}.");
    }
}
