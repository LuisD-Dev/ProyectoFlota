using System.ComponentModel.DataAnnotations;
namespace MantenimientoFlota.Domain.DTO.OrdenMantenimiento;

public sealed class CambiarEstadoOrdenDto
{
    [Required, Range(0, 7)]
    public int? Estado { get; set; }
    [Range(1, int.MaxValue)]
    public int? EspecialidadRequeridaId { get; set; }
    public DateTime? FechaProgramada { get; set; }
    [MaxLength(1000), RegularExpression(@"^[^<>]*$")]
    public string? Observacion { get; set; }
}
