namespace MantenimientoFlota.Domain.Entities;

public class Proveedor
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Contacto { get; set; } = string.Empty;

    public string Direccion { get; set; } = string.Empty;

    public string CondicionesGenerales { get; set; } = string.Empty;

    public ICollection<RepuestoProveedor> RepuestoProveedores { get; set; } =
        new List<RepuestoProveedor>();
}
