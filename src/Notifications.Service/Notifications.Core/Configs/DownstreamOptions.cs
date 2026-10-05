namespace Notifications.Core.Configs;

/// <summary>
/// URLs base de los otros microservicios a los que llama Notificaciones.
/// </summary>
public class DownstreamOptions
{
    /// <summary>Microservicio de Billing: consulta la factura pagada para notificar al paciente.</summary>
    public string BillingBaseUrl { get; set; } = string.Empty;
}
