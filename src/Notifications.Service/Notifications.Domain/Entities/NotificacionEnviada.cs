namespace Notifications.Domain.Entities;

/// <summary>
/// Registro de una notificación enviada. Es el resultado de confirmar el pago de una
/// factura: identifica el mensaje entregado al paciente.
/// </summary>
public class NotificacionEnviada
{
    public int NotificacionId { get; set; }
    public int FacturaId { get; set; }
    public string Destinatario { get; set; } = string.Empty;
    public string Asunto { get; set; } = string.Empty;
    public string MessageId { get; set; } = string.Empty;
    public DateTime EnviadaUtc { get; set; }
}
