using MantenimientoFlota.Api.Common;
using MantenimientoFlota.Api.Contracts;
using MantenimientoFlota.Application.Common.Results;
using MantenimientoFlota.Application.DTOs.Activo;
using MantenimientoFlota.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace MantenimientoFlota.Api.Controllers;

[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
[ApiController]
[Route("api/[controller]")]
public sealed class ActivoController : ControllerBase
{
    private readonly IActivoService _service;

    public ActivoController(IActivoService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ActivoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<ActivoDto>>> GetAllAsync(
        [FromQuery] ConsultarActivosDto consulta)
    {
        var activos = await _service.GetAllAsync(consulta.CategoriaActivoId, consulta.Estado);

        return Ok(activos);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ActivoDetalleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ActivoDetalleDto>> GetById(int id)
    {
        if (id <= 0)
        {
            return BadRequest(ApiErrorFactory.Create(HttpContext, StatusCodes.Status400BadRequest, "El identificador debe ser mayor que cero."));
        }

        var activo = await _service.GetByIdAsync(id);

        if (activo is null)
        {
            return NotFound(ApiErrorFactory.Create(HttpContext, StatusCodes.Status404NotFound, $"No se encontró el activo con id {id}."));
        }

        return Ok(activo);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ActivoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ActivoDto>> CreateAsync(CrearActivoDto dto)
    {
        var result = await _service.CreateAsync(dto);

        if (result.Status == ActivoOperationStatus.CodeAlreadyExists)
        {
            return Conflict(ApiErrorFactory.Create(HttpContext, StatusCodes.Status409Conflict, "Ya existe un activo con el código indicado."));
        }

        if (result.Status == ActivoOperationStatus.CategoryDoesNotExist)
        {
            return BadRequest(ApiErrorFactory.Create(HttpContext, StatusCodes.Status400BadRequest, "No existe una categoría de activo con el identificador indicado."));
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value!.Id },
            result.Value);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateAsync(
        int id,
        ActualizarActivoDto dto)
    {
        if (id <= 0)
        {
            return BadRequest(ApiErrorFactory.Create(HttpContext, StatusCodes.Status400BadRequest, "El identificador debe ser mayor que cero."));
        }

        var result = await _service.UpdateAsync(id, dto);

        if (result.Status == ActivoOperationStatus.DoesNotExist)
        {
            return NotFound(ApiErrorFactory.Create(HttpContext, StatusCodes.Status404NotFound, $"No se encontró el activo con id {id}."));
        }

        if (result.Status == ActivoOperationStatus.CodeAlreadyExists)
        {
            return Conflict(ApiErrorFactory.Create(HttpContext, StatusCodes.Status409Conflict, "Ya existe un activo con el código indicado."));
        }

        if (result.Status == ActivoOperationStatus.CategoryDoesNotExist)
        {
            return BadRequest(ApiErrorFactory.Create(HttpContext, StatusCodes.Status400BadRequest, "No existe una categoría de activo con el identificador indicado."));
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [EndpointDescription("Realiza la baja lógica del activo, conservando el registro y estableciendo su estado en DadoDeBaja. Devuelve 409 si tiene cualquier orden de mantenimiento asociada.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        if (id <= 0)
        {
            return BadRequest(ApiErrorFactory.Create(HttpContext, StatusCodes.Status400BadRequest, "El identificador debe ser mayor que cero."));
        }

        var result = await _service.DeleteAsync(id);

        if (result.Status == ActivoOperationStatus.DoesNotExist)
        {
            return NotFound(ApiErrorFactory.Create(HttpContext, StatusCodes.Status404NotFound, $"No se encontró el activo con id {id}."));
        }

        if (result.Status == ActivoOperationStatus.HasAssociatedMaintenanceOrders)
        {
            return Conflict(ApiErrorFactory.Create(HttpContext, StatusCodes.Status409Conflict, "No se puede eliminar el activo porque tiene órdenes de mantenimiento asociadas."));
        }

        return NoContent();
    }
}
