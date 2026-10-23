namespace MantenimientoFlota.Domain.DTO.OrdenMantenimiento;

/// <summary>Contrato de categoria orden resumen.</summary>
public sealed class CategoriaOrdenResumenDto
{
    /// <summary>Identificador asignado al recurso.</summary>
    /// <example>1</example>
    public int Id { get; set; }
    /// <summary>Nombre.</summary>
    /// <example>Vehículo de servicio</example>
    public string Nombre { get; set; } = string.Empty;
}
