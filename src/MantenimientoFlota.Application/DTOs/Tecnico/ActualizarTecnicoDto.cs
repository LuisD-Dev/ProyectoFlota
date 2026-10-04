using System.ComponentModel.DataAnnotations;

namespace MantenimientoFlota.Application.DTOs.Tecnico;

public sealed class ActualizarTecnicoDto
{
    [Required(ErrorMessage = "El nombre completo es obligatorio.")]
    [MaxLength(
        150,
        ErrorMessage = "El nombre completo no puede superar los 150 caracteres.")]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "La identificación es obligatoria.")]
    [MaxLength(
        50,
        ErrorMessage = "La identificación no puede superar los 50 caracteres.")]
    public string Identificacion { get; set; } = string.Empty;

    [MaxLength(
        30,
        ErrorMessage = "El teléfono no puede superar los 30 caracteres.")]
    public string? Telefono { get; set; }

    [MaxLength(
        254,
        ErrorMessage = "El correo no puede superar los 254 caracteres.")]
    public string? Correo { get; set; }

    [Required(ErrorMessage = "La disponibilidad es obligatoria.")]
    public bool? Disponible { get; set; }

    [Required(ErrorMessage = "La tarifa por hora es obligatoria.")]
    [Range(
        typeof(decimal),
        "-9999999999999999.99",
        "9999999999999999.99",
        ParseLimitsInInvariantCulture = true,
        ConvertValueInInvariantCulture = true,
        ErrorMessage = "La tarifa por hora está fuera del rango permitido.")]
    public decimal? TarifaPorHora { get; set; }
}
