namespace MantenimientoFlota.Domain.Entities;

public sealed class EstadoOrdenCatalogo
{
    public int Id { get; set; }
    public int Codigo { get; set; }
    public string Nombre { get; set; } = string.Empty;
}
