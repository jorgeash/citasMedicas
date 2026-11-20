namespace Notification.Core.Exceptions;

public enum Errores
{
    DATOS_INVALIDOS = 1201,
    NOTIFICACION_NO_ENCONTRADA = 1202,
    TIPO_INVALIDO = 1203,
    CANAL_INVALIDO = 1204,
    DESTINATARIO_INVALIDO = 1205,
    ESTADO_INVALIDO = 1206,
    ESTADO_NO_ENVIABLE = 1207,
    ESTADO_NO_LEIBLE = 1208,
    ESTADO_NO_CANCELABLE = 1209,
    MOTIVO_OBLIGATORIO = 1210,
    FECHA_PROGRAMADA_INVALIDA = 1211,
    PRIORIDAD_INVALIDA = 1212,
    ORIGEN_INVALIDO = 1213,
    EVENTO_INVALIDO = 1214,
    ERROR_ENVIO_OBLIGATORIO = 1215,
    TOKEN_EXPIRADO = 1290,
    TOKEN_INVALIDO = 1291,
    SIN_PERMISOS = 1292
}

public class DomainException : Exception
{
    public Errores Codigo { get; }

    public DomainException(Errores codigo, string message) : base(message)
    {
        Codigo = codigo;
    }
}
