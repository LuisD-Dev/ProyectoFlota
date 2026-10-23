using System.ComponentModel.DataAnnotations;

namespace MantenimientoFlota.Domain.DTO.RepuestoProveedor;

/// <summary>Contrato de crear repuesto proveedor.</summary>
public sealed class CrearRepuestoProveedorDto
{
    /// <summary>Identificador del recurso asociado; debe corresponder a un registro existente.</summary>
    /// <example>1</example>
    [Required(ErrorMessage = "El proveedor es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser mayor que cero.")]
    public int? ProveedorId { get; set; }

    /// <summary>Precio ofrecido por el proveedor para el repuesto.</summary>
    /// <example>1250</example>
    [Required(ErrorMessage = "El precio ofrecido es obligatorio.")]
    [Range(
        typeof(decimal),
        "0",
        "9999999999999999.99",
        ParseLimitsInInvariantCulture = true,
        ConvertValueInInvariantCulture = true,
        ErrorMessage = "El precio ofrecido debe ser mayor o igual que cero.")]
    public decimal? PrecioOfrecido { get; set; }

    /// <summary>Plazo de entrega expresado en días.</summary>
    /// <example>3</example>
    [Required(ErrorMessage = "El tiempo de entrega es obligatorio.")]
    [Range(
        0,
        int.MaxValue,
        ErrorMessage = "El tiempo de entrega debe ser mayor o igual que cero.")]
    public int? TiempoEntregaDias { get; set; }
}
