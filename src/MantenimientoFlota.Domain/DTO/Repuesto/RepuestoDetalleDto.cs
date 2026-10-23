namespace MantenimientoFlota.Domain.DTO.Repuesto;

/// <summary>Contrato de repuesto detalle.</summary>
public sealed class RepuestoDetalleDto
{
    /// <summary>Identificador asignado al recurso.</summary>
    /// <example>1</example>
    public int Id { get; set; }
    /// <summary>Código identificador del recurso.</summary>
    /// <example>REP-001</example>
    public string Codigo { get; set; } = string.Empty;
    /// <summary>Nombre.</summary>
    /// <example>Vehículo de servicio</example>
    public string Nombre { get; set; } = string.Empty;
    /// <summary>Unidad utilizada para medir las existencias.</summary>
    /// <example>Unidad</example>
    public string UnidadMedida { get; set; } = string.Empty;
    /// <summary>Cantidad disponible del repuesto.</summary>
    /// <example>5</example>
    public decimal StockActual { get; set; }
    /// <summary>Cantidad mínima requerida del repuesto.</summary>
    /// <example>10</example>
    public decimal StockMinimo { get; set; }
    /// <summary>Indica si el stock actual es inferior al mínimo.</summary>
    /// <example>true</example>
    public bool StockBajo { get; set; }
    /// <summary>Cantidad necesaria para alcanzar el stock mínimo.</summary>
    /// <example>5</example>
    public decimal FaltanteStockMinimo { get; set; }
    /// <summary>Número total de proveedores asociados.</summary>
    /// <example>2</example>
    public int CantidadProveedores { get; set; }
    /// <summary>Proveedores asociados al repuesto.</summary>
    public IReadOnlyList<ProveedorRepuestoResumenDto> Proveedores { get; set; } =
        new List<ProveedorRepuestoResumenDto>();
}
