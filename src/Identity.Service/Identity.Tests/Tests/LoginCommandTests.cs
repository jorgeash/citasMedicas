using Identity.Core.Exceptions;
using Identity.Core.Features.Auth.Commands.Login;

namespace Identity.Tests.Tests;

public class LoginCommandTests
{
    [Fact]
    public async Task Login_ConCredencialesValidas_DevuelveTokensYPerfil()
    {
        var escenario = new Fakes.EscenarioAuth();
        escenario.SembrarAdmin();

        var handler = new LoginCommandHandler(
            escenario.Usuarios, escenario.Sesiones, escenario.Permisos, escenario.Hasher, escenario.Tokens);

        var resultado = await handler.Handle(
            new LoginCommand { NombreUsuario = "admin", Password = "Admin123!" },
            CancellationToken.None);

        Assert.False(string.IsNullOrWhiteSpace(resultado.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(resultado.RefreshToken));
        Assert.Equal(3600, resultado.ExpiresIn);
        Assert.Equal(1, resultado.Usuario.UsuarioID);
        Assert.Equal("Administrador", resultado.Usuario.Rol);
        Assert.Contains("facturas.leer", resultado.Usuario.Permisos);
        Assert.Contains("usuarios.gestionar", resultado.Usuario.Permisos);
    }

    [Fact]
    public async Task Login_ConPasswordIncorrecta_LanzaCredencialesInvalidas()
    {
        var escenario = new Fakes.EscenarioAuth();
        escenario.SembrarAdmin();

        var handler = new LoginCommandHandler(
            escenario.Usuarios, escenario.Sesiones, escenario.Permisos, escenario.Hasher, escenario.Tokens);

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(
            new LoginCommand { NombreUsuario = "admin", Password = "OtraClave!" },
            CancellationToken.None));

        Assert.Equal(Errores.CREDENCIALES_INVALIDAS, ex.Codigo);
        Assert.Equal(701, (int)ex.Codigo);
    }

    [Fact]
    public async Task Login_ConUsuarioInexistente_DevuelveElMismoErrorQuePasswordMala()
    {
        // No se revela si la cuenta existe: mismo código y mismo mensaje.
        var escenario = new Fakes.EscenarioAuth();
        escenario.SembrarAdmin();

        var handler = new LoginCommandHandler(
            escenario.Usuarios, escenario.Sesiones, escenario.Permisos, escenario.Hasher, escenario.Tokens);

        var exUsuario = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(
            new LoginCommand { NombreUsuario = "nadie", Password = "x" },
            CancellationToken.None));

        var exPassword = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(
            new LoginCommand { NombreUsuario = "admin", Password = "x" },
            CancellationToken.None));

        Assert.Equal(exUsuario.Codigo, exPassword.Codigo);
        Assert.Equal(exUsuario.Message, exPassword.Message);
    }

    [Fact]
    public async Task Login_UsuarioInactivo_LanzaUsuarioInactivo()
    {
        var escenario = new Fakes.EscenarioAuth();
        var admin = escenario.SembrarAdmin();
        admin.Activo = false;

        var handler = new LoginCommandHandler(
            escenario.Usuarios, escenario.Sesiones, escenario.Permisos, escenario.Hasher, escenario.Tokens);

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(
            new LoginCommand { NombreUsuario = "admin", Password = "Admin123!" },
            CancellationToken.None));

        Assert.Equal(Errores.USUARIO_INACTIVO, ex.Codigo);
        Assert.Equal(702, (int)ex.Codigo);
    }

    [Fact]
    public async Task Login_SinUsuarioOSinPassword_LanzaDatosInvalidos()
    {
        var escenario = new Fakes.EscenarioAuth();
        escenario.SembrarAdmin();

        var handler = new LoginCommandHandler(
            escenario.Usuarios, escenario.Sesiones, escenario.Permisos, escenario.Hasher, escenario.Tokens);

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(
            new LoginCommand { NombreUsuario = "", Password = "" },
            CancellationToken.None));

        Assert.Equal(708, (int)ex.Codigo);
    }

    [Fact]
    public async Task Login_EscribeLaBitacoraDeSesionYActualizaElUltimoAcceso()
    {
        var escenario = new Fakes.EscenarioAuth();
        escenario.SembrarAdmin();

        var handler = new LoginCommandHandler(
            escenario.Usuarios, escenario.Sesiones, escenario.Permisos, escenario.Hasher, escenario.Tokens);

        await handler.Handle(
            new LoginCommand
            {
                NombreUsuario = "admin",
                Password = "Admin123!",
                DireccionIP = "127.0.0.1",
                Dispositivo = "Mozilla/5.0"
            },
            CancellationToken.None);

        var sesion = Assert.Single(escenario.Sesiones.Items);
        Assert.True(sesion.Activa);
        Assert.Equal(1, sesion.UsuarioID);
        Assert.Equal("127.0.0.1", sesion.DireccionIP);
        Assert.Equal("Mozilla/5.0", sesion.Dispositivo);
        Assert.NotNull(escenario.Usuarios.Items.Single().UltimoAcceso);
    }

    [Fact]
    public async Task Login_EsInsensibleAMayusculasDelNombreDeUsuario()
    {
        var escenario = new Fakes.EscenarioAuth();
        escenario.SembrarAdmin();

        var handler = new LoginCommandHandler(
            escenario.Usuarios, escenario.Sesiones, escenario.Permisos, escenario.Hasher, escenario.Tokens);

        var resultado = await handler.Handle(
            new LoginCommand { NombreUsuario = "  ADMIN  ", Password = "Admin123!" },
            CancellationToken.None);

        Assert.Equal("admin", resultado.Usuario.NombreUsuario);
    }
}
