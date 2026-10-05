namespace Notifications.Core.Exceptions;

public enum Errores
{
    FACTURA_NO_PAGADA = 801,
    CORREO_NO_ENVIADO = 802,
    FACTURA_NO_ENCONTRADA = 803,
    SERVICIO_NO_DISPONIBLE = 704
}

/// <summary>
/// Error de negocio con código propio; el middleware de la API lo transforma en una
/// respuesta 400 con el sobre HttpResponse en lugar de un 500 genérico.
/// </summary>
public class DomainException : Exception
{
    public Errores Codigo { get; }

    public DomainException(Errores codigo, string message) : base(message)
    {
        Codigo = codigo;
    }
}
