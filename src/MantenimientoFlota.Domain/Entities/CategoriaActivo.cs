namespace MantenimientoFlota.Domain.Entities;

public class CategoriaActivo
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    public int IntervaloMantenimientoKm { get; set; }

    public ICollection<Activo> Activos { get; set; } = new List<Activo>();
}
