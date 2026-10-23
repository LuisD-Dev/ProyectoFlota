using System.ComponentModel.DataAnnotations;
using MantenimientoFlota.Domain.DTO.Consultas;

namespace MantenimientoFlota.Domain.DTO.Activo;

/// <summary>Búsqueda opcional de activos con filtros, orden y paginación.</summary>
public sealed class BuscarActivosDto : ConsultaPaginadaDto, IValidatableObject
{
    /// <summary>Texto contenido en código, nombre o placa.</summary>
    /// <example>Hilux</example>
    [StringLength(150)]
    public string? Texto { get; set; }
    /// <summary>Categoría del activo. Omitir para consultar todas.</summary>
    /// <example>1</example>
    [Range(1, int.MaxValue)]
    public int? CategoriaActivoId { get; set; }
    /// <summary>Estado: 0 Operativo, 1 EnMantenimiento, 2 FueraDeServicio, 3 DadoDeBaja. Omitir incluye todos.</summary>
    /// <example>0</example>
    [Range(0, 3)]
    public int? Estado { get; set; }
    /// <summary>Kilometraje mínimo inclusive.</summary>
    /// <example>0</example>
    [Range(0, int.MaxValue)]
    public int? KilometrajeDesde { get; set; }
    /// <summary>Kilometraje máximo inclusive.</summary>
    /// <example>100000</example>
    [Range(0, int.MaxValue)]
    public int? KilometrajeHasta { get; set; }
    /// <summary>Campo de orden: nombre, codigo, fecha o kilometraje.</summary>
    /// <example>nombre</example>
    [Required, RegularExpression("(?i)^(nombre|codigo|fecha|kilometraje)$")]
    public string SortBy { get; set; } = "nombre";

    /// <summary>Valida la coherencia del intervalo de kilometraje.</summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (KilometrajeDesde.HasValue && KilometrajeHasta.HasValue && KilometrajeDesde > KilometrajeHasta)
            yield return new ValidationResult("El kilometraje mínimo no puede superar el máximo.",
                [nameof(KilometrajeDesde), nameof(KilometrajeHasta)]);
    }
}
