using System.ComponentModel.DataAnnotations;

namespace MantenimientoFlota.Domain.DTO.Consultas;

/// <summary>Parámetros comunes de una consulta paginada.</summary>
public abstract class ConsultaPaginadaDto
{
    /// <summary>Número de página, desde uno.</summary>
    /// <example>1</example>
    [Range(1, int.MaxValue)]
    public int PageNumber { get; set; } = 1;
    /// <summary>Registros por página, entre uno y cincuenta.</summary>
    /// <example>10</example>
    [Range(1, 50)]
    public int PageSize { get; set; } = 10;
    /// <summary>Dirección del orden: asc o desc.</summary>
    /// <example>asc</example>
    [Required, RegularExpression("(?i)^(asc|desc)$")]
    public string SortDirection { get; set; } = "asc";
}
