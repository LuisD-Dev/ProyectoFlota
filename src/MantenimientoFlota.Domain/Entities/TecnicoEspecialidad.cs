namespace MantenimientoFlota.Domain.Entities;

public class TecnicoEspecialidad
{
    public int TecnicoId { get; set; }

    public int EspecialidadId { get; set; }

    public string NivelCertificacion { get; set; } = string.Empty;

    public DateOnly FechaObtencion { get; set; }

    public Tecnico Tecnico { get; set; } = null!;

    public Especialidad Especialidad { get; set; } = null!;
}
