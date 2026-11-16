namespace Billing.Core.Exceptions;

public enum Errores
{
    PAGO_FALLIDO = 601,
    PASARELA_NO_DISPONIBLE = 701,
    AUTENTICACION_FALLIDA = 702,

    FACTURA_SIN_DETALLE = 801,
    CANTIDAD_INVALIDA = 802,
    PACIENTE_INVALIDO = 803,
    PACIENTE_NO_ENCONTRADO = 804,
    TARIFA_NO_VALIDA = 805,
    FACTURA_NO_ENCONTRADA = 806,
    FACTURA_YA_PAGADA = 807,
    FACTURA_YA_ANULADA = 808,
    CITA_NO_ENCONTRADA = 809,
    CITA_PACIENTE_INCONSISTENTE = 810
}

/// <summary>Error de negocio con código propio; el middleware de la API lo
/// transforma en una respuesta 400 con el sobre HttpResponse.</summary>
public class DomainException : Exception
{
    public Errores Codigo { get; }

    public DomainException(Errores codigo, string message) : base(message)
    {
        Codigo = codigo;
    }
}
