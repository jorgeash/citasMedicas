namespace Identity.Domain.Entities;

/// <summary>Tabla puente rol↔permiso. Mapea <c>dbo.Rol_Permiso</c>.</summary>
public class RolPermiso
{
    public int RolID { get; set; }
    public int PermisoID { get; set; }

    public Rol? Rol { get; set; }
    public Permiso? Permiso { get; set; }
}
