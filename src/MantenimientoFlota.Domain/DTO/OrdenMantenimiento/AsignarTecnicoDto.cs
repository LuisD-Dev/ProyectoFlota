using System.ComponentModel.DataAnnotations;
namespace MantenimientoFlota.Domain.DTO.OrdenMantenimiento;

public sealed class AsignarTecnicoDto
{
    [Range(1, int.MaxValue)]
    public int TecnicoId { get; set; }
    [Range(typeof(decimal), "0.01", "1000000", ParseLimitsInInvariantCulture = true)]
    public decimal HorasTrabajadas { get; set; }
}
