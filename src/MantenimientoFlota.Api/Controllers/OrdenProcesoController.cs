using MantenimientoFlota.Api.Authorization;
using MantenimientoFlota.Api.Common;
using MantenimientoFlota.Application.Interfaces;
using MantenimientoFlota.Domain.DTO.OrdenMantenimiento;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace MantenimientoFlota.Api.Controllers;

[ApiController]
[Route("api/OrdenMantenimiento/{id:int}")]
public sealed class OrdenProcesoController(IOrdenProcesoService service) : ControllerBase
{
    [HttpPatch("estado")]
    [Authorize(Policy = AuthorizationPolicies.AdministradorOCoordinador)]
    public async Task<IActionResult> CambiarEstadoAsync(int id, CambiarEstadoOrdenDto dto) =>
        this.FromResult(await service.CambiarEstadoAsync(id, dto));

    [HttpPost("repuestos")]
    [Authorize(Policy = AuthorizationPolicies.AdministradorOCoordinador)]
    public async Task<IActionResult> RegistrarRepuestosAsync(int id, RegistrarRepuestosDto dto) =>
        this.FromResult(await service.RegistrarRepuestosAsync(id, dto));

    [HttpPost("tecnicos")]
    [Authorize(Policy = AuthorizationPolicies.AdministradorOCoordinador)]
    public async Task<IActionResult> AsignarTecnicoAsync(int id, AsignarTecnicoDto dto) =>
        this.FromResult(await service.AsignarTecnicoAsync(id, dto));

    [HttpGet("repuestos")]
    [Authorize(Policy = AuthorizationPolicies.TodosLosRoles)]
    public async Task<IActionResult> GetRepuestosAsync(int id) => this.FromResult(await service.GetRepuestosAsync(id));

    [HttpGet("tecnicos")]
    [Authorize(Policy = AuthorizationPolicies.TodosLosRoles)]
    public async Task<IActionResult> GetTecnicosAsync(int id) => this.FromResult(await service.GetTecnicosAsync(id));

    [HttpGet("historial")]
    [Authorize(Policy = AuthorizationPolicies.TodosLosRoles)]
    public async Task<IActionResult> GetHistorialAsync(int id) => this.FromResult(await service.GetHistorialAsync(id));

    [HttpGet("/api/Activo/{id:int}/historial")]
    [Authorize(Policy = AuthorizationPolicies.TodosLosRoles)]
    public async Task<IActionResult> GetHistorialActivoAsync(int id) => this.FromResult(await service.GetHistorialActivoAsync(id));
}
