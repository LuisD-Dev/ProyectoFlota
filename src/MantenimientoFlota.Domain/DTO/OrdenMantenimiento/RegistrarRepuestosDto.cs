using System.ComponentModel.DataAnnotations;
namespace MantenimientoFlota.Domain.DTO.OrdenMantenimiento;

public sealed class RegistrarRepuestosDto
{
    [Required, MinLength(1), MaxLength(50)]
    public List<RepuestoSolicitadoDto> Repuestos { get; set; } = [];
}

public sealed class RepuestoSolicitadoDto
{
    [Range(1, int.MaxValue)]
    public int RepuestoId { get; set; }
    [Range(typeof(decimal), "0.01", "9999999999999999.99", ParseLimitsInInvariantCulture = true)]
    public decimal Cantidad { get; set; }
}
