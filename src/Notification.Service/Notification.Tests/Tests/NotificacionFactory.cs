using Notification.Domain.Entities;

namespace Notification.Tests.Tests;

internal static class NotificacionFactory
{
    internal static Notificacion Crear(
        int notificacionID = 1,
        string eventoID = "EVT-1",
        long usuarioID = 1,
        string estado = "PENDIENTE",
        string tipo = "CITA",
        string canal = "INTERNO",
        string prioridad = "NORMAL",
        string origenServicio = "SCHEDULING",
        string? destinatario = null,
        string titulo = "Confirmación de cita",
        string mensaje = "Tiene una cita programada.",
        string? referencia = null,
        bool activo = true,
        DateTime? fechaCreacion = null,
        DateTime? fechaLectura = null)
    {
        var fecha = fechaCreacion ?? new DateTime(2026, 1, 10, 8, 0, 0, DateTimeKind.Utc);

        return new Notificacion
        {
            NotificacionID = notificacionID,
            EventoID = eventoID,
            UsuarioID = usuarioID,
            Destinatario = destinatario,
            Titulo = titulo,
            Mensaje = mensaje,
            Tipo = tipo,
            Canal = canal,
            Prioridad = prioridad,
            Estado = estado,
            OrigenServicio = origenServicio,
            Referencia = referencia,
            FechaCreacion = fecha,
            FechaLectura = fechaLectura,
            Intentos = 0,
            Activo = activo,
            CreatedAt = fecha
        };
    }
}
