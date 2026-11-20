namespace Notification.Core.DTOs.Notification;

public class NotificacionDto
{
    public int NotificacionID { get; set; }
    public string EventoID { get; set; } = string.Empty;
    public long UsuarioID { get; set; }
    public string? Destinatario { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Canal { get; set; } = string.Empty;
    public string Prioridad { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string OrigenServicio { get; set; } = string.Empty;
    public string? Referencia { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaProgramada { get; set; }
    public DateTime? FechaEnvio { get; set; }
    public DateTime? FechaLectura { get; set; }
    public DateTime? FechaCancelacion { get; set; }
    public string? MotivoCancelacion { get; set; }
    public int Intentos { get; set; }
    public string? UltimoError { get; set; }
    public bool Activo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
