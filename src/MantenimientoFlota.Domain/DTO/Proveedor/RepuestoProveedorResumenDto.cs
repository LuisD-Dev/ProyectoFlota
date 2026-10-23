namespace MantenimientoFlota.Domain.DTO.Proveedor;

/// <summary>Contrato de repuesto proveedor resumen.</summary>
public sealed class RepuestoProveedorResumenDto
{
    /// <summary>Identificador del recurso asociado; debe corresponder a un registro existente.</summary>
    /// <example>1</example>
    public int RepuestoId { get; set; }
    /// <summary>Código identificador del recurso.</summary>
    /// <example>REP-001</example>
    public string Codigo { get; set; } = string.Empty;
    /// <summary>Nombre.</summary>
    /// <example>Vehículo de servicio</example>
    public string Nombre { get; set; } = string.Empty;
    /// <summary>Precio ofrecido por el proveedor para el repuesto.</summary>
    /// <example>1250</example>
    public decimal PrecioOfrecido { get; set; }
    /// <summary>Plazo de entrega expresado en días.</summary>
    /// <example>3</example>
    public int TiempoEntregaDias { get; set; }
}
