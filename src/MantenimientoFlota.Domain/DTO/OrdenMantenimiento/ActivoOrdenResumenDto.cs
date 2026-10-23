using MantenimientoFlota.Domain.Constants;

namespace MantenimientoFlota.Domain.DTO.OrdenMantenimiento;

/// <summary>Contrato de activo orden resumen.</summary>
public sealed class ActivoOrdenResumenDto
{
    /// <summary>Identificador asignado al recurso.</summary>
    /// <example>1</example>
    public int Id { get; set; }
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
    /// <summary>Categoría asociada al activo.</summary>
    public CategoriaOrdenResumenDto Categoria { get; set; } = new();
}
