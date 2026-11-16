using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Identity.Core.Services;
using Identity.Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Tests.Tests;

public class TokenServiceTests
{
    [Fact]
    public void CreateAccessToken_DevuelveTokenConLaVidaConfigurada()
    {
        var escenario = new Fakes.EscenarioAuth();
        var admin = escenario.Usuarios.Items.Single();

        var (token, expiresIn) = escenario.Tokens.CreateAccessToken(admin, new[] { "facturas.leer" });

        Assert.False(string.IsNullOrWhiteSpace(token));
        Assert.Equal(60 * 60, expiresIn);   // AccessTokenMinutes = 60
        Assert.Equal(3, token.Split('.').Length);
    }

    [Fact]
    public void CreateAccessToken_IncluyeRolYPermisosComoClaims()
    {
        var escenario = new Fakes.EscenarioAuth();
        escenario.SembrarAdmin();
        var admin = escenario.Usuarios.Items.Single();

        var (token, _) = escenario.Tokens.CreateAccessToken(
            admin, new[] { "facturas.leer", "usuarios.gestionar" });

        var claims = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.ToList();

        Assert.Equal(admin.UsuarioID.ToString(), claims.Single(c => c.Type == "sub").Value);
        Assert.Equal("access", claims.Single(c => c.Type == "typ").Value);
        Assert.Equal("admin", claims.Single(c => c.Type == "usuario").Value);
        Assert.Equal("Administrador", claims.Single(c => c.Type == "rol").Value);
        Assert.Equal(2, claims.Count(c => c.Type == "permiso"));
    }

    [Fact]
    public void CreateRefreshToken_LlevaElTipoRefresh()
    {
        var escenario = new Fakes.EscenarioAuth();
        var admin = escenario.Usuarios.Items.Single();

        var token = escenario.Tokens.CreateRefreshToken(admin);
        var claims = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims;

        Assert.Equal("refresh", claims.Single(c => c.Type == "typ").Value);
    }

    [Fact]
    public void ValidateRefreshToken_ConTokenValido_DevuelveElUsuario()
    {
        var escenario = new Fakes.EscenarioAuth();
        var admin = escenario.Usuarios.Items.Single();

        var token = escenario.Tokens.CreateRefreshToken(admin);
        var usuario = escenario.Tokens.ValidateRefreshToken(token);

        Assert.NotNull(usuario);
        Assert.Equal(admin.UsuarioID, usuario!.UsuarioID);
    }

    [Fact]
    public void ValidateRefreshToken_RechazaUnAccessToken()
    {
        // Un access token no puede canjearse por otro: solo sirve el de refresh.
        var escenario = new Fakes.EscenarioAuth();
        var admin = escenario.Usuarios.Items.Single();

        var (accessToken, _) = escenario.Tokens.CreateAccessToken(admin, Array.Empty<string>());

        Assert.Null(escenario.Tokens.ValidateRefreshToken(accessToken));
    }

    [Fact]
    public void ValidateRefreshToken_RechazaTokenFirmadoConOtraClave()
    {
        var escenario = new Fakes.EscenarioAuth();
        var admin = escenario.Usuarios.Items.Single();
        var token = escenario.Tokens.CreateRefreshToken(admin);

        var ajeno = new Fakes.EscenarioAuth(o => o.SigningKey =
            "OtraClaveDistintaDe32BytesMinimoAAAAAAAAAAAAAA=");

        Assert.Null(ajeno.Tokens.ValidateRefreshToken(token));
    }

    [Fact]
    public void ValidateRefreshToken_RechazaTokenVencido()
    {
        var escenario = new Fakes.EscenarioAuth();
        var admin = escenario.Usuarios.Items.Single();

        // Un refresh real emitido con fecha de expiración en el pasado.
        var vencido = EmitirRefresh(admin, notBefore: DateTime.UtcNow.AddDays(-10), expires: DateTime.UtcNow.AddDays(-1));

        Assert.Null(escenario.Tokens.ValidateRefreshToken(vencido));
    }

    /// <summary>Emite un JWT de refresh a mano, para poder controlar su vigencia.</summary>
    private static string EmitirRefresh(Usuario usuario, DateTime notBefore, DateTime expires)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.UsuarioID.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("typ", "refresh"),
            new("usuario", usuario.NombreUsuario)
        };

        var credenciales = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Fakes.EscenarioAuth.ClaveJwt)),
            SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: "CitasMedicas.Identity",
            audience: "CitasMedicas.Frontend",
            claims: claims,
            notBefore: notBefore,
            expires: expires,
            signingCredentials: credenciales);

        return new JwtSecurityTokenHandler { MapInboundClaims = false }.WriteToken(jwt);
    }

    [Fact]
    public void ValidateRefreshToken_RechazaBasuraOVacio()
    {
        var escenario = new Fakes.EscenarioAuth();

        Assert.Null(escenario.Tokens.ValidateRefreshToken(""));
        Assert.Null(escenario.Tokens.ValidateRefreshToken("   "));
        Assert.Null(escenario.Tokens.ValidateRefreshToken("no.es.un.jwt"));
        Assert.Null(escenario.Tokens.ValidateRefreshToken(
            "eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiIxIn0.firma-falsa"));
    }

    [Fact]
    public void CreateAccessToken_ConUnMinutoDeVida_ExpiraEnSesentaSegundos()
    {
        var escenario = new Fakes.EscenarioAuth(o => o.AccessTokenMinutes = 1);
        var admin = escenario.Usuarios.Items.Single();

        var (_, expiresIn) = escenario.Tokens.CreateAccessToken(admin, Array.Empty<string>());

        Assert.Equal(60, expiresIn);
    }
}
