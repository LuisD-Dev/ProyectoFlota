using MantenimientoFlota.Domain.Constants;

namespace MantenimientoFlota.Domain.DTO.CategoriaActivo;

public sealed class ActivoResumenDto
{
    public string Codigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    [System.ComponentModel.Description("Estado del activo: 0 Operativo, 1 EnMantenimiento, 2 FueraDeServicio, 3 DadoDeBaja.")]
    public int Estado { get; set; }
}
