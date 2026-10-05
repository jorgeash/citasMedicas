using Notifications.Core.Exceptions;
using Notifications.Core.Interfaces.Services;
using Notifications.Domain.Entities;
using Serilog;

namespace Notifications.Infraestructure.Services;

/// <summary>
/// Envío de correo simulado: no abre conexión SMTP, solo deja traza en Serilog con el mismo
/// mensaje que enviaría y devuelve un MessageId. Sustituirlo por un remitente real es
/// cambiar esta clase, sin tocar Core.
/// </summary>
internal class SimulatedEmailSender : IEmailSender
{
    public Task<NotificacionEnviada> EnviarAsync(int facturaId, string destinatario, string asunto, string cuerpo, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(destinatario) || !destinatario.Contains('@'))
            throw new DomainException(Errores.CORREO_NO_ENVIADO,
                $"El destinatario '{destinatario}' no es un correo válido.");

        var notificacion = new NotificacionEnviada
        {
            NotificacionId = Environment.TickCount,
            FacturaId = facturaId,
            Destinatario = destinatario,
            Asunto = asunto,
            MessageId = $"<sim-{Guid.NewGuid():N}@notificaciones.local>",
            EnviadaUtc = DateTime.UtcNow
        };

        Log.ForContext("MessageId", notificacion.MessageId)
           .ForContext("FacturaId", facturaId)
           .ForContext("Destinatario", destinatario)
           .ForContext("Asunto", asunto)
           .Information("[correo simulado] {Cuerpo}", cuerpo);

        return Task.FromResult(notificacion);
    }
}
