using Notifications.Domain.Entities;

namespace Notifications.Core.Interfaces.Services;

/// <summary>
/// Envía el correo al paciente. La implementación real usaría un SMTP o un proveedor de
/// correo; aquí el envío es simulado.
/// </summary>
public interface IEmailSender
{
    Task<NotificacionEnviada> EnviarAsync(int facturaId, string destinatario, string asunto, string cuerpo, CancellationToken cancellationToken = default);
}
