using MantenimientoFlota.Domain.Enums;

namespace MantenimientoFlota.Application.DTOs.OrdenMantenimiento;

public sealed class ActivoOrdenResumenDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public EstadoActivo Estado { get; set; }
    public CategoriaOrdenResumenDto Categoria { get; set; } = new();
}
