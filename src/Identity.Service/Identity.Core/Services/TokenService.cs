using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Identity.Core.DTOs.Identity;
using Identity.Core.Interfaces;
using Identity.Core.Options;
using Identity.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Core.Services;

/// <summary>
/// Emite y valida JWT firmados con HMAC-SHA256. Es stateless: no guarda estado en
/// BD y no consulta <c>Sesiones_Usuario</c> (esa tabla es solo bitácora).
/// </summary>
public class TokenService : ITokenService
{
    private const string TipoAcceso = "access";
    private const string TipoRefresh = "refresh";

    private readonly JwtOptions _opciones;

    public TokenService(IOptions<JwtOptions> opciones)
    {
        _opciones = opciones.Value;
    }

    public (string Token, int ExpiresIn) CreateAccessToken(Usuario usuario, IReadOnlyCollection<string> permisos)
    {
        ArgumentNullException.ThrowIfNull(usuario);

        var ahora = DateTime.UtcNow;
        var expira = ahora.AddMinutes(_opciones.AccessTokenMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.UsuarioID.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("typ", TipoAcceso),
            new("usuario", usuario.NombreUsuario),
            new("nombre", usuario.NombreCompleto),
            new("rol", usuario.Rol?.Nombre ?? string.Empty)
        };

        if (usuario.SucursalID.HasValue)
            claims.Add(new Claim("sucursal", usuario.SucursalID.Value.ToString()));

        foreach (var codigo in permisos)
            claims.Add(new Claim("permiso", codigo));

        var token = Emitir(claims, ahora, expira);
        var segundos = (int)(expira - ahora).TotalSeconds;
        return (token, segundos);
    }

    public string CreateRefreshToken(Usuario usuario)
    {
        ArgumentNullException.ThrowIfNull(usuario);

        var ahora = DateTime.UtcNow;
        var expira = ahora.AddDays(_opciones.RefreshTokenDays);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.UsuarioID.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("typ", TipoRefresh),
            new("usuario", usuario.NombreUsuario)
        };

        return Emitir(claims, ahora, expira);
    }

    public Usuario? ValidateRefreshToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        var parametros = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _opciones.Issuer,
            ValidateAudience = true,
            ValidAudience = _opciones.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opciones.SigningKey)),
            ValidateLifetime = true,
            // Un minuto de margen: evita rechazar un token recién emitido por el reloj.
            ClockSkew = TimeSpan.FromMinutes(1)
        };

        try
        {
            // MapInboundClaims=false conserva los claims tal cual se emitieron
            // ("sub" no se convierte en ClaimTypes.NameIdentifier), así el claim
            // "typ" y el id del usuario se leen por su nombre literal.
            var handler = new JwtSecurityTokenHandler { MapInboundClaims = false };
            var principal = handler.ValidateToken(token, parametros, out var validado);

            // Solo sirve un refresh: un access token no puede canjearse por otro.
            if (validado is not JwtSecurityToken jwt ||
                !string.Equals(ObtenerClaim(principal, "typ"), TipoRefresh, StringComparison.Ordinal))
            {
                return null;
            }

            var sub = ObtenerClaim(principal, JwtRegisteredClaimNames.Sub);
            if (!int.TryParse(sub, out var usuarioID))
                return null;

            return new Usuario { UsuarioID = usuarioID };
        }
        catch (Exception ex) when (
            ex is SecurityTokenException or      // firma, audiencia, emisor o vida inválidos
            ArgumentException or                 // estructura de segmentos rota
            FormatException or                   // base64 ilegible en el header/payload
            InvalidOperationException)           // algoritmo/clave no soportados
        {
            // El caller traduce esto a 704 (refresh inválido o vencido).
            return null;
        }
    }

    private string Emitir(IEnumerable<Claim> claims, DateTime desde, DateTime hasta)
    {
        var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opciones.SigningKey));
        var credenciales = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: _opciones.Issuer,
            audience: _opciones.Audience,
            claims: claims,
            notBefore: desde,
            expires: hasta,
            signingCredentials: credenciales);

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    private static string? ObtenerClaim(ClaimsPrincipal principal, string tipo)
        => principal.FindFirst(tipo)?.Value;
}
