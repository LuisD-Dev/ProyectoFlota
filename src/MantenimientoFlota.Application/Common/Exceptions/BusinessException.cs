namespace MantenimientoFlota.Application.Common.Exceptions;

// El mensaje debe ser público y controlado; no incluir detalles técnicos.
public abstract class BusinessException : Exception
{
    protected BusinessException(string message) : base(message) { }
}
