namespace MantenimientoFlota.Infrastructure.Exceptions;

// El repositorio solo utiliza esta excepción cuando SQL ha rechazado la escritura.
public sealed class ImagenUpdateRejectedException(Exception innerException)
    : Exception("La actualización de la imagen fue rechazada.", innerException);
