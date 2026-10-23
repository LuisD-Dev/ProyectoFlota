using MantenimientoFlota.Domain.DTO.PatternResult;
using MantenimientoFlota.Api.Common;
using System.ComponentModel.DataAnnotations;
using MantenimientoFlota.Api.Contracts;

using MantenimientoFlota.Domain.DTO.Activo;
using MantenimientoFlota.Application.Interfaces;
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

    /// <summary>Buscar activos con filtros, orden y paginación.</summary>
    /// <param name="logger">Registrador de la operación, resuelto por inyección de dependencias.</param>
    /// <param name="consulta">Filtros de consulta.</param>
    /// <response code="200">Operación completada; el cuerpo contiene Result.</response>
    /// <response code="400">Entrada inválida; consulte los errores por campo.</response>
    [EndpointSummary("Buscar activos con filtros, orden y paginación")]
    [EndpointDescription("Página inicial 1, tamaño 10 y máximo 50. Orden por lista blanca y desempate por ID. Sin coincidencias devuelve 200 con data vacía. Los filtros omitidos no restringen el resultado.")]
    [HttpGet("buscar")]
    [ProducesResponseType(typeof(Result<IReadOnlyList<ActivoDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result<IReadOnlyList<ActivoDto>>>> SearchAsync(
        [FromServices] ILogger<ActivoController> logger,
        [FromQuery] BuscarActivosDto consulta)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Activo.SearchAsync");
        return this.FromResult(await _service.SearchAsync(consulta));
    }

    /// <summary>Listar activos.</summary>
    /// <param name="logger">Registrador de la operación, resuelto por inyección de dependencias.</param>
    /// <param name="consulta">Filtros de consulta.</param>
    /// <response code="200">Operación completada; el cuerpo contiene Result.</response>
    /// <response code="400">Entrada inválida; consulte los errores por campo.</response>
    [EndpointSummary("Listar activos")]
    [HttpGet]
    [ProducesResponseType(typeof(Result<IEnumerable<ActivoDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result<IEnumerable<ActivoDto>>>> GetAllAsync(
        [FromServices] ILogger<ActivoController> logger,
        [FromQuery] ConsultarActivosDto consulta)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Activo.GetAllAsync");
        var activos = await _service.GetAllAsync(consulta.CategoriaActivoId, consulta.Estado);

        return this.FromResult(activos);
    }

    /// <summary>Consultar detalle de activos.</summary>
    /// <param name="logger">Registrador de la operación, resuelto por inyección de dependencias.</param>
    /// <param name="id">Identificador positivo del recurso.</param>
    /// <response code="200">Operación completada; el cuerpo contiene Result.</response>
    /// <response code="400">Entrada inválida; consulte los errores por campo.</response>
    /// <response code="404">El recurso solicitado no existe.</response>
    [EndpointSummary("Consultar detalle de activos")]
    [HttpGet("{id:int}", Name = "Activo_GetById")]
    [ProducesResponseType(typeof(Result<ActivoDetalleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<ActivoDetalleDto>>> GetByIdAsync(
        [FromServices] ILogger<ActivoController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int id)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Activo.GetById");

        var activo = await _service.GetByIdAsync(id);

        return this.FromResult(activo);
    }

    /// <summary>Registrar activos.</summary>
    /// <param name="logger">Registrador de la operación, resuelto por inyección de dependencias.</param>
    /// <param name="dto">Datos de entrada de la operación.</param>
    /// <response code="201">Recurso creado; Location permite consultar su detalle.</response>
    /// <response code="400">Entrada inválida; consulte los errores por campo.</response>
    /// <response code="409">La operación entra en conflicto con los datos o las reglas de negocio.</response>
    /// <response code="415">El tipo de contenido no está admitido.</response>
    [EndpointSummary("Registrar activos")]
    [HttpPost]
    [ProducesResponseType(typeof(Result<ActivoDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public async Task<ActionResult<Result<ActivoDto>>> CreateAsync(
        [FromServices] ILogger<ActivoController> logger,
        CrearActivoDto dto)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Activo.CreateAsync");
        var result = await _service.CreateAsync(dto);

        if (!result.IsSuccess) return this.FromResult(result);

        return CreatedAtRoute(
            "Activo_GetById",
            new { id = result.Data!.Id },
            result);
    }

    /// <summary>Actualizar activos.</summary>
    /// <param name="logger">Registrador de la operación, resuelto por inyección de dependencias.</param>
    /// <param name="id">Identificador positivo del recurso.</param>
    /// <param name="dto">Datos de entrada de la operación.</param>
    /// <response code="200">Operación completada; el cuerpo contiene Result.</response>
    /// <response code="400">Entrada inválida; consulte los errores por campo.</response>
    /// <response code="404">El recurso solicitado no existe.</response>
    /// <response code="409">La operación entra en conflicto con los datos o las reglas de negocio.</response>
    /// <response code="415">El tipo de contenido no está admitido.</response>
    [EndpointSummary("Actualizar activos")]
    [HttpPut("{id:int}")]
    [EndpointDescription("Actualiza el activo. Si cambia a FueraDeServicio o DadoDeBaja, rechaza la solicitud con 409 cuando existen órdenes abiertas. Reenviar el estado actual permite actualizar los demás campos.")]
    [ProducesResponseType(typeof(Result<object?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public async Task<IActionResult> UpdateAsync(
        [FromServices] ILogger<ActivoController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int id,
        ActualizarActivoDto dto)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Activo.UpdateAsync");

        var result = await _service.UpdateAsync(id, dto);

        return this.FromResult(result);
    }

    /// <summary>Cargar o reemplazar imagen de activo.</summary>
    /// <param name="logger">Registrador de la operación, resuelto por inyección de dependencias.</param>
    /// <param name="id">Identificador positivo del recurso.</param>
    /// <param name="dto">Datos de entrada de la operación.</param>
    /// <param name="cancellationToken">Parámetro de la operación.</param>
    /// <response code="200">Operación completada; el cuerpo contiene Result.</response>
    /// <response code="400">Entrada inválida; consulte los errores por campo.</response>
    /// <response code="404">El recurso solicitado no existe.</response>
    /// <response code="413">El tamaño de la solicitud supera el límite permitido.</response>
    /// <response code="415">El tipo de contenido no está admitido.</response>
    [EndpointSummary("Cargar o reemplazar imagen de activo")]
    [HttpPut("{id:int}/imagen")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(3 * 1024 * 1024)]
    [RequestFormLimits(MultipartBodyLengthLimit = 3 * 1024 * 1024)]
    [EndpointDescription("Carga o reemplaza una imagen JPEG/PNG de máximo 2 MiB. Valida extensión y firma, sin decodificación completa. Límites del servidor pueden producir 413 antes del controlador.")]
    [ProducesResponseType(typeof(Result<ActivoImagenDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status413PayloadTooLarge)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public async Task<IActionResult> ActualizarImagenAsync(
        [FromServices] ILogger<ActivoController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int id, [FromForm] CargarImagenActivoDto dto,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Activo.ActualizarImagenAsync");

        var archivo = await ActivoImagenValidator.LeerAsync(dto.Archivo!, cancellationToken);
        if (archivo.Error is not null)
            return BadRequest(ApiErrorFactory.Create(HttpContext, 400, archivo.Error,
                new Dictionary<string, string[]> { [nameof(dto.Archivo)] = [archivo.Error] }));

        using var contenido = archivo.Contenido!;
        var result = await _service.ActualizarImagenAsync(id, contenido, archivo.Extension!, cancellationToken);

        return this.FromResult(result);
    }

    /// <summary>Cambiar estado administrativo del activo.</summary>
    /// <param name="logger">Registrador de la operación, resuelto por inyección de dependencias.</param>
    /// <param name="id">Identificador positivo del recurso.</param>
    /// <param name="dto">Datos de entrada de la operación.</param>
    /// <response code="200">Operación completada; el cuerpo contiene Result.</response>
    /// <response code="400">Entrada inválida; consulte los errores por campo.</response>
    /// <response code="404">El recurso solicitado no existe.</response>
    /// <response code="409">La operación entra en conflicto con los datos o las reglas de negocio.</response>
    /// <response code="415">El tipo de contenido no está admitido.</response>
    [EndpointSummary("Cambiar estado administrativo del activo")]
    [HttpPatch("{id:int}/estado")]
    [EndpointDescription("Establece exclusivamente FueraDeServicio (2) o DadoDeBaja (3). Rechaza con 409 si existen órdenes abiertas, incluso al repetir el estado actual. Las órdenes finales no bloquean esta operación.")]
    [ProducesResponseType(typeof(Result<object?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public async Task<IActionResult> CambiarEstadoAdministrativoAsync(
        [FromServices] ILogger<ActivoController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int id,
        CambiarEstadoActivoDto dto)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Activo.CambiarEstadoAdministrativoAsync");

        var result = await _service.CambiarEstadoAdministrativoAsync(id, dto);

        return this.FromResult(result);
    }

    /// <summary>Dar de baja lógica un activo.</summary>
    /// <param name="logger">Registrador de la operación, resuelto por inyección de dependencias.</param>
    /// <param name="id">Identificador positivo del recurso.</param>
    /// <response code="200">Operación completada; el cuerpo contiene Result.</response>
    /// <response code="400">Entrada inválida; consulte los errores por campo.</response>
    /// <response code="404">El recurso solicitado no existe.</response>
    /// <response code="409">La operación entra en conflicto con los datos o las reglas de negocio.</response>
    [EndpointSummary("Dar de baja lógica un activo")]
    [HttpDelete("{id:int}")]
    [EndpointDescription("Realiza la baja lógica del activo, conservando el registro y estableciendo su estado en DadoDeBaja. Devuelve 409 si tiene cualquier orden de mantenimiento asociada.")]
    [ProducesResponseType(typeof(Result<object?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteAsync(
        [FromServices] ILogger<ActivoController> logger,
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")] int id)
    {
        logger.LogInformation("Operación invocada. Operation={Operation}", "Activo.DeleteAsync");

        var result = await _service.DeleteAsync(id);

        return this.FromResult(result);
    }
}
