using System.ComponentModel.DataAnnotations;
using MantenimientoFlota.Domain.DTO.Consultas;

namespace MantenimientoFlota.Domain.DTO.Repuesto;

/// <summary>Búsqueda de repuestos por texto, stock y proveedor, con orden y paginación.</summary>
public sealed class BuscarRepuestosDto : ConsultaPaginadaDto, IValidatableObject
{
    /// <summary>Texto contenido en código o nombre.</summary>
    /// <example>Filtro</example>
    [StringLength(150)]
    public string? Texto { get; set; }
    /// <summary>Proveedor asociado. Omitir para consultar todos.</summary>
    /// <example>1</example>
    [Range(1, int.MaxValue)]
    public int? ProveedorId { get; set; }
    /// <summary>True: stock bajo el mínimo. False: stock igual o superior. Omitir incluye ambos.</summary>
    /// <example>true</example>
    public bool? StockBajo { get; set; }
    /// <summary>Stock actual mínimo inclusive.</summary>
    /// <example>0</example>
    [Range(typeof(decimal), "0", "9999999999999999.99", ParseLimitsInInvariantCulture = true)]
    public decimal? StockDesde { get; set; }
    /// <summary>Stock actual máximo inclusive.</summary>
    /// <example>100</example>
    [Range(typeof(decimal), "0", "9999999999999999.99", ParseLimitsInInvariantCulture = true)]
    public decimal? StockHasta { get; set; }
    /// <summary>Campo de orden: nombre, codigo, stock o minimo.</summary>
    /// <example>stock</example>
    [Required, RegularExpression("(?i)^(nombre|codigo|stock|minimo)$")]
    public string SortBy { get; set; } = "nombre";

    /// <summary>Valida la coherencia del intervalo de stock.</summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StockDesde.HasValue && StockHasta.HasValue && StockDesde > StockHasta)
            yield return new ValidationResult("El stock mínimo de búsqueda no puede superar el máximo.",
                [nameof(StockDesde), nameof(StockHasta)]);
    }
}
