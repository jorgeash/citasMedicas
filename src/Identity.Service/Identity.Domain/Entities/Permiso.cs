namespace Identity.Domain.Entities;

/// <summary>Permiso del catálogo. Mapea <c>dbo.Permisos</c>.</summary>
public class Permiso
{
    public int PermisoID { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}
