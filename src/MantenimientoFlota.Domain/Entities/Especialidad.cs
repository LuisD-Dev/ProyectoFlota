namespace MantenimientoFlota.Domain.Entities;

public class Especialidad
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    public ICollection<TecnicoEspecialidad> TecnicoEspecialidades { get; set; } =
        new List<TecnicoEspecialidad>();
}
