namespace MantenimientoFlota.Domain.Constants;

/// <summary>Valores numéricos estables del estado; no son un enum.</summary>
public static class EstadoActivo
{
    public const int Operativo = 0;
    public const int EnMantenimiento = 1;
    public const int FueraDeServicio = 2;
    public const int DadoDeBaja = 3;
}
