using MantenimientoFlota.Domain.Constants;

namespace MantenimientoFlota.Domain.Entities;

public class Activo
{
    public int Id { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    public DateOnly FechaAdquisicion { get; set; }

    public decimal ValorReferencia { get; set; }

    public int Estado { get; set; }

    public string ImagenUrl { get; set; } = string.Empty;

    public string Placa { get; set; } = string.Empty;

    public int KilometrajeActual { get; set; }

    public int CategoriaActivoId { get; set; }

    public CategoriaActivo CategoriaActivo { get; set; } = null!;

    public ICollection<OrdenMantenimiento> OrdenesMantenimiento { get; set; } =
        new List<OrdenMantenimiento>();
}
