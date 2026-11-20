using Notification.Core.DTOs.Notification;
using Notification.Domain.Entities;

namespace Notification.Core.Features.Notification;

internal static class NotificationMapper
{
    public static NotificacionDto ToDto(Notificacion notificacion)
    {
        return new NotificacionDto
        {
            NotificacionID = notificacion.NotificacionID,
            EventoID = notificacion.EventoID,
            UsuarioID = notificacion.UsuarioID,
            Destinatario = notificacion.Destinatario,
            Titulo = notificacion.Titulo,
            Mensaje = notificacion.Mensaje,
            Tipo = notificacion.Tipo,
            Canal = notificacion.Canal,
            Prioridad = notificacion.Prioridad,
            Estado = notificacion.Estado,
            OrigenServicio = notificacion.OrigenServicio,
            Referencia = notificacion.Referencia,
            FechaCreacion = notificacion.FechaCreacion,
            FechaProgramada = notificacion.FechaProgramada,
            FechaEnvio = notificacion.FechaEnvio,
            FechaLectura = notificacion.FechaLectura,
            FechaCancelacion = notificacion.FechaCancelacion,
            MotivoCancelacion = notificacion.MotivoCancelacion,
            Intentos = notificacion.Intentos,
            UltimoError = notificacion.UltimoError,
            Activo = notificacion.Activo,
            CreatedAt = notificacion.CreatedAt,
            UpdatedAt = notificacion.UpdatedAt
        };
    }

    public static string? Truncar(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var trimmed = value.Trim();
        return trimmed.Length <= maxLength ? trimmed : trimmed[..maxLength];
    }
}
