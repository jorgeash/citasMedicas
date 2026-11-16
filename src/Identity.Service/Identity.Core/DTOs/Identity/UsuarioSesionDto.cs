namespace Identity.Core.DTOs.Identity;

/// <summary>Perfil del usuario autenticado, tal lo devuelve <c>GET /api/auth/me</c>.</summary>
public class UsuarioSesionDto
{
    public int UsuarioID { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Rol { get; set; } = string.Empty;
    public int? SucursalID { get; set; }
    public List<string> Permisos { get; set; } = new();
}

/// <summary>Resultado de <c>login</c> y <c>refresh</c>.</summary>
public class AuthResultDto
{
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>Segundos de vida del access token, para que el cliente programe el refresh.</summary>
    public int ExpiresIn { get; set; }

    public string RefreshToken { get; set; } = string.Empty;
    public UsuarioSesionDto Usuario { get; set; } = new();
}
