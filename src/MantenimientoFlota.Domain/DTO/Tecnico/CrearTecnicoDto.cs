using System.ComponentModel.DataAnnotations;

namespace MantenimientoFlota.Domain.DTO.Tecnico;

/// <summary>Contrato de crear tecnico.</summary>
public sealed class CrearTecnicoDto
{
    /// <summary>Nombre completo del técnico.</summary>
    /// <example>María Rodríguez</example>
    [Required(ErrorMessage = "El nombre completo es obligatorio.")]
    [MaxLength(
        150,
        ErrorMessage = "El nombre completo no puede superar los 150 caracteres.")]
    public string NombreCompleto { get; set; } = string.Empty;

    /// <summary>Identificación del técnico.</summary>
    /// <example>ID-DEMO-001</example>
    [Required(ErrorMessage = "La identificación es obligatoria.")]
    [MaxLength(
        50,
        ErrorMessage = "La identificación no puede superar los 50 caracteres.")]
    public string Identificacion { get; set; } = string.Empty;

    /// <summary>Número de teléfono de contacto.</summary>
    /// <example>8888-0000</example>
    [MaxLength(
        30,
        ErrorMessage = "El teléfono no puede superar los 30 caracteres.")]
    public string? Telefono { get; set; }

    /// <summary>Correo electrónico de contacto.</summary>
    /// <example>demo@example.test</example>
    [MaxLength(
        254,
        ErrorMessage = "El correo no puede superar los 254 caracteres.")]
    public string? Correo { get; set; }

    /// <summary>Indica si el técnico está disponible.</summary>
    /// <example>true</example>
    [Required(ErrorMessage = "La disponibilidad es obligatoria.")]
    public bool? Disponible { get; set; }

    /// <summary>Tarifa del técnico por hora.</summary>
    /// <example>15000</example>
    [Required(ErrorMessage = "La tarifa por hora es obligatoria.")]
    [Range(
        typeof(decimal),
        "0",
        "9999999999999999.99",
        ParseLimitsInInvariantCulture = true,
        ConvertValueInInvariantCulture = true,
        ErrorMessage = "La tarifa por hora debe ser mayor o igual que cero.")]
    public decimal? TarifaPorHora { get; set; }
}
