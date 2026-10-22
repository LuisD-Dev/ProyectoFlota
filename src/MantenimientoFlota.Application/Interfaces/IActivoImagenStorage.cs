namespace MantenimientoFlota.Application.Interfaces;

public interface IActivoImagenStorage
{
    Task<string> GuardarAsync(int activoId, Stream contenido, string extension, CancellationToken cancellationToken);
    bool EsRutaAdministrada(int activoId, string ruta);
    bool EsRutaReservada(string? ruta);
    Task EliminarSiAdministradaAsync(int activoId, string ruta);
}
