using System.ComponentModel.DataAnnotations;
using MantenimientoFlota.Domain.Enums;

namespace MantenimientoFlota.Application.DTOs.Activo;

public sealed class CambiarEstadoActivoDto
{
    [Required(ErrorMessage = "El estado es obligatorio.")]
    [EnumDataType(typeof(EstadoActivo), ErrorMessage = "El estado indicado no es válido.")]
    public EstadoActivo? Estado { get; set; }
}
