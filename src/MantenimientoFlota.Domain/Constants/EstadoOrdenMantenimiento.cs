namespace MantenimientoFlota.Domain.Constants;

/// <summary>Valores numéricos estables del estado; no son un enum.</summary>
public static class EstadoOrdenMantenimiento
{
    public const int Solicitada = 0;
    public const int Diagnosticada = 1;
    public const int Aprobada = 2;
    public const int Rechazada = 3;
    public const int EnEjecucion = 4;
    public const int EnEsperaDeRepuesto = 5;
    public const int Completada = 6;
    public const int Cancelada = 7;
}
