using System.ComponentModel.DataAnnotations;

namespace MantenimientoFlota.Domain.DTO.RepuestoProveedor;

public sealed class ActualizarRepuestoProveedorDto
{
    [Required(ErrorMessage = "El precio ofrecido es obligatorio.")]
    [Range(
        typeof(decimal),
        "0",
        "9999999999999999.99",
        ParseLimitsInInvariantCulture = true,
        ConvertValueInInvariantCulture = true,
        ErrorMessage = "El precio ofrecido debe ser mayor o igual que cero.")]
    public decimal? PrecioOfrecido { get; set; }

    [Required(ErrorMessage = "El tiempo de entrega es obligatorio.")]
    [Range(
        0,
        int.MaxValue,
        ErrorMessage = "El tiempo de entrega debe ser mayor o igual que cero.")]
    public int? TiempoEntregaDias { get; set; }
}
