namespace Lab.Core.Features.Lab;

internal static class Estados
{
    public const string SolicitudPendiente = "PENDIENTE";
    public const string SolicitudEnProceso = "EN_PROCESO";
    public const string SolicitudCompletada = "COMPLETADA";
    public const string SolicitudCancelada = "CANCELADA";
    public const string PrioridadNormal = "NORMAL";
    public const string PrioridadUrgente = "URGENTE";
    public const string ResultadoNormal = "NORMAL";
    public const string ResultadoAnormal = "ANORMAL";
    public const string ResultadoCritico = "CRITICO";

    public static readonly string[] TiposEstudio = { "LABORATORIO", "IMAGEN", "PATOLOGIA", "OTRO" };

    public static readonly string[] EstadosResultado =
    {
        ResultadoNormal,
        ResultadoAnormal,
        ResultadoCritico
    };
}
