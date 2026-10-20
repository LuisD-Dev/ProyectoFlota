using MantenimientoFlota.Domain.Enums;

namespace MantenimientoFlota.Application.DTOs.CategoriaActivo;

public sealed class ActivoResumenDto
{
    public string Codigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public EstadoActivo Estado { get; set; }
}
