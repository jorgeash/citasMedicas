namespace Business.Core.Exceptions;

public enum Errores
{
    CLIENTE_INVALIDO = 501,
    CODIGO_CLIENTE_DUPLICADO = 502,
    LOCATION_ID_INVALIDO = 503,
    LOCATION_NO_ENCONTRADA = 504,
    TIPO_CLIENTE_NO_ENCONTRADO = 505,
    EMPRESA_NO_ENCONTRADA = 506,
    CLIENTE_NO_ENCONTRADO = 507,
    CONTACTO_INVALIDO = 508,
    ADDRESSES_NO_DISPONIBLE = 509
}

/// <summary>
/// Error de negocio con codigo propio; el middleware de la API lo transforma en una
/// respuesta 400 con el sobre HttpResponse en lugar de un 500 generico.
/// </summary>
public class DomainException : Exception
{
    public Errores Codigo { get; }

    public DomainException(Errores codigo, string message) : base(message)
    {
        Codigo = codigo;
    }
}