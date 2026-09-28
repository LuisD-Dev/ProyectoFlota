using System.ComponentModel.DataAnnotations;
using MantenimientoFlota.Domain.Enums;

namespace MantenimientoFlota.Application.DTOs.Activo;

public sealed class ActualizarActivoDto
{
    [Required(ErrorMessage = "El código es obligatorio.")]
    [MaxLength(50, ErrorMessage = "El código no puede superar los 50 caracteres.")]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
    public string? Descripcion { get; set; }

    [Required(ErrorMessage = "La fecha de adquisición es obligatoria.")]
    public DateOnly? FechaAdquisicion { get; set; }

    [Required(ErrorMessage = "El valor de referencia es obligatorio.")]
    [Range(
        typeof(decimal),
        "0",
        "9999999999999999.99",
        ParseLimitsInInvariantCulture = true,
        ConvertValueInInvariantCulture = true,
        ErrorMessage = "El valor de referencia debe ser mayor o igual que cero.")]
    public decimal? ValorReferencia { get; set; }

    [Required(ErrorMessage = "El estado es obligatorio.")]
    [EnumDataType(
        typeof(EstadoActivo),
        ErrorMessage = "El estado indicado no es válido.")]
    public EstadoActivo? Estado { get; set; }

    [MaxLength(2048, ErrorMessage = "La URL de la imagen no puede superar los 2048 caracteres.")]
    public string? ImagenUrl { get; set; }

    [MaxLength(20, ErrorMessage = "La placa no puede superar los 20 caracteres.")]
    public string? Placa { get; set; }

    [Required(ErrorMessage = "El kilometraje actual es obligatorio.")]
    [Range(
        0,
        int.MaxValue,
        ErrorMessage = "El kilometraje actual debe ser mayor o igual que cero.")]
    public int? KilometrajeActual { get; set; }

    [Required(ErrorMessage = "La categoría del activo es obligatoria.")]
    [Range(
        1,
        int.MaxValue,
        ErrorMessage = "La categoría del activo debe ser válida.")]
    public int? CategoriaActivoId { get; set; }
}
