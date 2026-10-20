using System.ComponentModel.DataAnnotations;
using MantenimientoFlota.Domain.Enums;

namespace MantenimientoFlota.Application.DTOs.Activo;

public sealed class ConsultarActivosDto
{
    [Range(1, int.MaxValue,
        ErrorMessage = "El identificador de la categoría debe ser mayor que cero.")]
    public int? CategoriaActivoId { get; set; }

    [EnumDataType(typeof(EstadoActivo),
        ErrorMessage = "El estado indicado no es válido.")]
    public EstadoActivo? Estado { get; set; }
}
