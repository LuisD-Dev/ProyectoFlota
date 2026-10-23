using MantenimientoFlota.Domain.Constants;

namespace MantenimientoFlota.Domain.DTO.CategoriaActivo;

/// <summary>Contrato de activo resumen.</summary>
public sealed class ActivoResumenDto
{
    /// <summary>Código identificador del recurso.</summary>
    /// <example>VEH-001</example>
    public string Codigo { get; set; } = string.Empty;

    /// <summary>Nombre.</summary>
    /// <example>Vehículo de servicio</example>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Estado numérico del activo: 0 Operativo, 1 EnMantenimiento, 2 FueraDeServicio y 3 DadoDeBaja.</summary>
    /// <example>0</example>
    [System.ComponentModel.Description("Estado del activo: 0 Operativo, 1 EnMantenimiento, 2 FueraDeServicio, 3 DadoDeBaja.")]
    public int Estado { get; set; }
}
