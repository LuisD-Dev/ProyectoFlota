using MantenimientoFlota.Api.Authorization;
using MantenimientoFlota.Api.Common;
using MantenimientoFlota.Application.Interfaces;
using MantenimientoFlota.Domain.DTO.Usuario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace MantenimientoFlota.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = AuthorizationPolicies.SoloAdministrador)]
public sealed class UsuarioController(IUsuarioService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllAsync() => this.FromResult(await service.GetAllAsync());

    [HttpGet("{id:int}", Name = "Usuario_GetById")]
    public async Task<IActionResult> GetByIdAsync(int id) => this.FromResult(await service.GetByIdAsync(id));

    [HttpGet("roles")]
    public async Task<IActionResult> GetRolesAsync() => this.FromResult(await service.GetRolesAsync());

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CrearUsuarioDto dto)
    {
        var result = await service.CreateAsync(dto);
        return CreatedAtRoute("Usuario_GetById", new { id = result.Data!.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateAsync(int id, ActualizarUsuarioDto dto) =>
        this.FromResult(await service.UpdateAsync(id, dto));

    /// <summary>Desactivar sin eliminar el usuario ni su actividad.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DesactivarAsync(int id) => this.FromResult(await service.DesactivarAsync(id));
}
