using MantenimientoFlota.Api.Contracts;
using MantenimientoFlota.Application.Common.Exceptions;
using MantenimientoFlota.Application.DTOs.TecnicoEspecialidad;
using MantenimientoFlota.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace MantenimientoFlota.Api.Controllers;

[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
[ApiController]
[Route("api/[controller]")]
public sealed class TecnicoEspecialidadController : ControllerBase
{
    private readonly ITecnicoEspecialidadService _service;

    public TecnicoEspecialidadController(ITecnicoEspecialidadService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IEnumerable<TecnicoEspecialidadDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TecnicoEspecialidadDto>>> GetAllAsync()
    {
        var tecnicoEspecialidades = await _service.GetAllAsync();

        return Ok(tecnicoEspecialidades);
    }

    [HttpGet("{tecnicoId:int}/{especialidadId:int}")]
    [ProducesResponseType(typeof(TecnicoEspecialidadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TecnicoEspecialidadDto>> GetById(
        int tecnicoId,
        int especialidadId)
    {
        var tecnicoEspecialidad = await _service.GetByIdAsync(
            tecnicoId,
            especialidadId);

        return Ok(tecnicoEspecialidad);
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(TecnicoEspecialidadDto),
        StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TecnicoEspecialidadDto>> CreateAsync(
        CrearTecnicoEspecialidadDto dto)
    {
        var result = await _service.CreateAsync(dto);

        var tecnicoEspecialidad = result;

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                tecnicoId = tecnicoEspecialidad.TecnicoId,
                especialidadId = tecnicoEspecialidad.EspecialidadId
            },
            tecnicoEspecialidad);
    }

    [HttpPut("{tecnicoId:int}/{especialidadId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync(
        int tecnicoId,
        int especialidadId,
        ActualizarTecnicoEspecialidadDto dto)
    {
        await _service.UpdateAsync(
            tecnicoId,
            especialidadId,
            dto);

        return NoContent();
    }

    [HttpDelete("{tecnicoId:int}/{especialidadId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(
        int tecnicoId,
        int especialidadId)
    {
        await _service.DeleteAsync(tecnicoId, especialidadId);

        return NoContent();
    }
}
