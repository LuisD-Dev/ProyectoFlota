using MantenimientoFlota.Api.Common;
using MantenimientoFlota.Api.Contracts;
using MantenimientoFlota.Application.Common.Exceptions;
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
            throw new InvalidRequestException("El identificador debe ser mayor que cero.");
        }

        var activo = await _service.GetByIdAsync(id);

        return Ok(activo);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ActivoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ActivoDto>> CreateAsync(CrearActivoDto dto)
    {
        var result = await _service.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
    }

    [HttpPut("{id:int}")]
    [EndpointDescription("Actualiza el activo. Si cambia a FueraDeServicio o DadoDeBaja, rechaza la solicitud con 409 cuando existen órdenes abiertas. Reenviar el estado actual permite actualizar los demás campos.")]
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
            throw new InvalidRequestException("El identificador debe ser mayor que cero.");
        }

        await _service.UpdateAsync(id, dto);

        return NoContent();
    }

    [HttpPut("{id:int}/imagen")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(3 * 1024 * 1024)]
    [RequestFormLimits(MultipartBodyLengthLimit = 3 * 1024 * 1024)]
    [EndpointDescription("Carga o reemplaza una imagen JPEG/PNG de máximo 2 MiB. Valida extensión y firma, sin decodificación completa. Límites del servidor pueden producir 413 antes del controlador.")]
    [ProducesResponseType(typeof(ActivoImagenDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status413PayloadTooLarge)]
    public async Task<IActionResult> ActualizarImagenAsync(int id, [FromForm] CargarImagenActivoDto dto,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
            throw new InvalidRequestException("El identificador debe ser mayor que cero.");

        var archivo = await ActivoImagenValidator.LeerAsync(dto.Archivo!, cancellationToken);
        if (archivo.Error is not null)
            return BadRequest(ApiErrorFactory.Create(HttpContext, 400, archivo.Error,
                new Dictionary<string, string[]> { [nameof(dto.Archivo)] = [archivo.Error] }));

        using var contenido = archivo.Contenido!;
        var result = await _service.ActualizarImagenAsync(id, contenido, archivo.Extension!, cancellationToken);

        return Ok(result);
    }

    [HttpPatch("{id:int}/estado")]
    [EndpointDescription("Establece exclusivamente FueraDeServicio (2) o DadoDeBaja (3). Rechaza con 409 si existen órdenes abiertas, incluso al repetir el estado actual. Las órdenes finales no bloquean esta operación.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CambiarEstadoAdministrativoAsync(int id, CambiarEstadoActivoDto dto)
    {
        if (id <= 0)
        {
            throw new InvalidRequestException("El identificador debe ser mayor que cero.");
        }

        await _service.CambiarEstadoAdministrativoAsync(id, dto);

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
            throw new InvalidRequestException("El identificador debe ser mayor que cero.");
        }

        await _service.DeleteAsync(id);

        return NoContent();
    }
}
