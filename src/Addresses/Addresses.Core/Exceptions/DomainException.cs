namespace Addresses.Core.Exceptions;

public enum Errores
{
    PAIS_DUPLICADO = 401,
    PAIS_NO_ENCONTRADO = 402,
    ESTADO_NO_ENCONTRADO = 403,
    CIUDAD_NO_ENCONTRADA = 404,
    POSICION_NO_ENCONTRADA = 405,
    PAIS_INVALIDO = 406,
    CIUDAD_INVALIDA = 407,
    DIRECCION_INVALIDA = 408
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