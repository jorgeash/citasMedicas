namespace Identity.Domain.Entities;

/// <summary>
/// Registro de auditoría de acceso. Mapea <c>dbo.Sesiones_Usuario</c>.
/// Es solo bitácora: el esquema JWT es stateless y nunca consulta esta tabla
/// para validar un token.
/// </summary>
public class SesionUsuario
{
    public long SesionID { get; set; }
    public int UsuarioID { get; set; }
    public string? TokenSesion { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public string? DireccionIP { get; set; }
    public string? Dispositivo { get; set; }
    public bool Activa { get; set; }

    public Usuario? Usuario { get; set; }
}
