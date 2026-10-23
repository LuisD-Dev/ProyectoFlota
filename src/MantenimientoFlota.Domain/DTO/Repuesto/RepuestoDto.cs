namespace MantenimientoFlota.Domain.DTO.Repuesto;

/// <summary>Contrato de repuesto.</summary>
public sealed class RepuestoDto
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
}
