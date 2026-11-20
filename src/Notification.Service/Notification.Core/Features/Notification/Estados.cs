namespace Notification.Core.Features.Notification;

internal static class Estados
{
    public const string Pendiente = "PENDIENTE";
    public const string Enviada = "ENVIADA";
    public const string Leida = "LEIDA";
    public const string Fallida = "FALLIDA";
    public const string Cancelada = "CANCELADA";

    public static readonly string[] Tipos =
    {
        "CITA",
        "RECORDATORIO",
        "RESULTADO",
        "FACTURA",
        "PAGO",
        "HOSPITALIZACION",
        "SISTEMA"
    };

    public static readonly string[] Canales =
    {
        "INTERNO",
        "EMAIL",
        "SMS",
        "PUSH"
    };

    public static readonly string[] Prioridades =
    {
        "BAJA",
        "NORMAL",
        "ALTA",
        "CRITICA"
    };

    public static readonly string[] Origenes =
    {
        "IDENTITY",
        "SCHEDULING",
        "CLINICALRECORDS",
        "LABORATORY",
        "BILLING",
        "NOTIFICATION",
        "SISTEMA"
    };
}
