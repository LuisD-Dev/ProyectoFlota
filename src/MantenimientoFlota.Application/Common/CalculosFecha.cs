namespace MantenimientoFlota.Application.Common;

public static class CalculosFecha
{
    public static int CalcularAntiguedadAnios(DateOnly fechaAdquisicion, DateOnly fechaReferencia)
    {
        if (fechaAdquisicion > fechaReferencia)
            return 0;

        var anios = fechaReferencia.Year - fechaAdquisicion.Year;
        if (fechaAdquisicion.AddYears(anios) > fechaReferencia)
            anios--;

        return anios;
    }
}
