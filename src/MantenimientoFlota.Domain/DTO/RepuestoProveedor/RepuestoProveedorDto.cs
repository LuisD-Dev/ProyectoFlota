namespace MantenimientoFlota.Domain.DTO.RepuestoProveedor;

/// <summary>Contrato de repuesto proveedor.</summary>
public sealed class RepuestoProveedorDto
{
    /// <summary>Identificador del recurso asociado; debe corresponder a un registro existente.</summary>
    /// <example>1</example>
    public int RepuestoId { get; set; }

    /// <summary>Repuesto nombre.</summary>
    public string RepuestoNombre { get; set; } = string.Empty;

    /// <summary>Identificador del recurso asociado; debe corresponder a un registro existente.</summary>
    /// <example>1</example>
    public int ProveedorId { get; set; }

    /// <summary>Proveedor nombre.</summary>
    public string ProveedorNombre { get; set; } = string.Empty;

    /// <summary>Precio ofrecido por el proveedor para el repuesto.</summary>
    /// <example>1250</example>
    public decimal PrecioOfrecido { get; set; }

    /// <summary>Plazo de entrega expresado en días.</summary>
    /// <example>3</example>
    public int TiempoEntregaDias { get; set; }
}
