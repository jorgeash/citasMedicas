namespace Identity.Domain.Entities;

/// <summary>
/// Usuario del sistema. Mapea la tabla existente <c>dbo.Usuarios</c> de ClinicaDB;
/// Identity no crea ni altera esquema, solo lee y escribe estas filas.
/// </summary>
public class Usuario
{
    public int UsuarioID { get; set; }
    public int RolID { get; set; }
    public int? SucursalID { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public DateTime? UltimoAcceso { get; set; }
    public DateTime FechaCreacion { get; set; }

    public Rol? Rol { get; set; }
}
