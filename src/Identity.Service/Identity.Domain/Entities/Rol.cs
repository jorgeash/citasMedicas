namespace Identity.Domain.Entities;

/// <summary>Rol de usuario. Mapea <c>dbo.Roles</c>.</summary>
public class Rol
{
    public int RolID { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }

    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    public ICollection<Permiso> Permisos { get; set; } = new List<Permiso>();
}
