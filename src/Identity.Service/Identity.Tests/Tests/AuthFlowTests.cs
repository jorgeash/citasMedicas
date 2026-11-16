using Identity.Core.Exceptions;
using Identity.Core.Features.Auth.Commands.Login;
using Identity.Core.Features.Auth.Commands.Logout;
using Identity.Core.Features.Auth.Commands.Refresh;
using Identity.Core.Features.Auth.Queries.GetMe;

namespace Identity.Tests.Tests;

public class AuthFlowTests
{
    private static async Task<string> LoginAsync(Fakes.EscenarioAuth escenario, string usuario, string password)
    {
        var login = new LoginCommandHandler(
            escenario.Usuarios, escenario.Sesiones, escenario.Permisos, escenario.Hasher, escenario.Tokens);

        var resultado = await login.Handle(
            new LoginCommand { NombreUsuario = usuario, Password = password },
            CancellationToken.None);

        return resultado.RefreshToken;
    }

    // ------------------------------------------------------------ refresh

    [Fact]
    public async Task Refresh_ConTokenValido_EmiteUnParDeTokensNuevo()
    {
        var escenario = new Fakes.EscenarioAuth();
        escenario.SembrarAdmin();
        var refreshToken = await LoginAsync(escenario, "admin", "Admin123!");

        var handler = new RefreshCommandHandler(
            escenario.Usuarios, escenario.Sesiones, escenario.Permisos, escenario.Tokens);

        var resultado = await handler.Handle(
            new RefreshCommand { RefreshToken = refreshToken }, CancellationToken.None);

        Assert.False(string.IsNullOrWhiteSpace(resultado.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(resultado.RefreshToken));
        Assert.Equal(1, resultado.Usuario.UsuarioID);
        Assert.Equal("admin", resultado.Usuario.NombreUsuario);
    }

    [Fact]
    public async Task Refresh_ConTokenBasura_LanzaRefreshInvalido()
    {
        var escenario = new Fakes.EscenarioAuth();
        escenario.SembrarAdmin();

        var handler = new RefreshCommandHandler(
            escenario.Usuarios, escenario.Sesiones, escenario.Permisos, escenario.Tokens);

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(
            new RefreshCommand { RefreshToken = "esto-no-es-un-token" },
            CancellationToken.None));

        Assert.Equal(703, (int)ex.Codigo);
    }

    [Fact]
    public async Task Refresh_SinToken_LanzaRefreshInvalido()
    {
        var escenario = new Fakes.EscenarioAuth();
        escenario.SembrarAdmin();

        var handler = new RefreshCommandHandler(
            escenario.Usuarios, escenario.Sesiones, escenario.Permisos, escenario.Tokens);

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(
            new RefreshCommand { RefreshToken = "" }, CancellationToken.None));

        Assert.Equal(703, (int)ex.Codigo);
    }

    [Fact]
    public async Task Refresh_ConUsuarioDesactivado_LanzaUsuarioInactivo()
    {
        var escenario = new Fakes.EscenarioAuth();
        var admin = escenario.SembrarAdmin();
        var refreshToken = await LoginAsync(escenario, "admin", "Admin123!");

        admin.Activo = false;

        var handler = new RefreshCommandHandler(
            escenario.Usuarios, escenario.Sesiones, escenario.Permisos, escenario.Tokens);

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(
            new RefreshCommand { RefreshToken = refreshToken }, CancellationToken.None));

        Assert.Equal(702, (int)ex.Codigo);
    }

    // ------------------------------------------------------------- logout

    [Fact]
    public async Task Logout_CierraLasSesionesAbiertasDelUsuario()
    {
        var escenario = new Fakes.EscenarioAuth();
        escenario.SembrarAdmin();
        await LoginAsync(escenario, "admin", "Admin123!");
        await LoginAsync(escenario, "admin", "Admin123!");

        Assert.Equal(2, escenario.Sesiones.Items.Count(s => s.Activa));

        var handler = new LogoutCommandHandler(escenario.Sesiones);
        await handler.Handle(new LogoutCommand { UsuarioID = 1 }, CancellationToken.None);

        Assert.All(escenario.Sesiones.Items, s =>
        {
            Assert.False(s.Activa);
            Assert.NotNull(s.FechaFin);
        });
    }

    [Fact]
    public async Task Logout_SinSesionesAbiertas_NoFalla()
    {
        var escenario = new Fakes.EscenarioAuth();
        escenario.SembrarAdmin();

        var handler = new LogoutCommandHandler(escenario.Sesiones);
        await handler.Handle(new LogoutCommand { UsuarioID = 1 }, CancellationToken.None);
    }

    [Fact]
    public async Task Logout_SinUsuario_LanzaNoAutenticado()
    {
        var escenario = new Fakes.EscenarioAuth();

        var handler = new LogoutCommandHandler(escenario.Sesiones);

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(
            new LogoutCommand { UsuarioID = 0 }, CancellationToken.None));

        Assert.Equal(704, (int)ex.Codigo);
    }

    // ---------------------------------------------------------------- me

    [Fact]
    public async Task GetMe_DevuelveElPerfilConRolYPermisosOrdenados()
    {
        var escenario = new Fakes.EscenarioAuth();
        escenario.SembrarAdmin();

        var handler = new GetMeQueryHandler(escenario.Usuarios, escenario.Permisos);

        var perfil = await handler.Handle(new GetMeQuery { UsuarioID = 1 }, CancellationToken.None);

        Assert.Equal(1, perfil.UsuarioID);
        Assert.Equal("admin", perfil.NombreUsuario);
        Assert.Equal("Administrador", perfil.Rol);
        Assert.Equal(1, perfil.SucursalID);
        Assert.Equal(new[] { "facturas.leer", "usuarios.gestionar" }, perfil.Permisos);
    }

    [Fact]
    public async Task GetMe_UsuarioInexistente_LanzaNoAutenticado()
    {
        var escenario = new Fakes.EscenarioAuth();
        escenario.SembrarAdmin();

        var handler = new GetMeQueryHandler(escenario.Usuarios, escenario.Permisos);

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(
            new GetMeQuery { UsuarioID = 999 }, CancellationToken.None));

        Assert.Equal(704, (int)ex.Codigo);
    }

    [Fact]
    public async Task GetMe_SinId_LanzaNoAutenticado()
    {
        var escenario = new Fakes.EscenarioAuth();

        var handler = new GetMeQueryHandler(escenario.Usuarios, escenario.Permisos);

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(
            new GetMeQuery { UsuarioID = 0 }, CancellationToken.None));

        Assert.Equal(704, (int)ex.Codigo);
    }

    [Fact]
    public async Task GetMe_UsuarioInactivo_LanzaUsuarioInactivo()
    {
        var escenario = new Fakes.EscenarioAuth();
        var admin = escenario.SembrarAdmin();
        admin.Activo = false;

        var handler = new GetMeQueryHandler(escenario.Usuarios, escenario.Permisos);

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(
            new GetMeQuery { UsuarioID = 1 }, CancellationToken.None));

        Assert.Equal(702, (int)ex.Codigo);
    }

    // ------------------------------------------- flujo completo login→me

    [Fact]
    public async Task FlujoCompleto_Login_Refresh_Me_Logout()
    {
        var escenario = new Fakes.EscenarioAuth();
        escenario.SembrarAdmin();

        var login = new LoginCommandHandler(
            escenario.Usuarios, escenario.Sesiones, escenario.Permisos, escenario.Hasher, escenario.Tokens);
        var refresh = new RefreshCommandHandler(
            escenario.Usuarios, escenario.Sesiones, escenario.Permisos, escenario.Tokens);
        var me = new GetMeQueryHandler(escenario.Usuarios, escenario.Permisos);
        var logout = new LogoutCommandHandler(escenario.Sesiones);

        var sesion = await login.Handle(
            new LoginCommand { NombreUsuario = "admin", Password = "Admin123!" },
            CancellationToken.None);

        var renovada = await refresh.Handle(
            new RefreshCommand { RefreshToken = sesion.RefreshToken }, CancellationToken.None);

        var perfil = await me.Handle(
            new GetMeQuery { UsuarioID = renovada.Usuario.UsuarioID }, CancellationToken.None);

        await logout.Handle(new LogoutCommand { UsuarioID = perfil.UsuarioID }, CancellationToken.None);

        Assert.Equal("admin", perfil.NombreUsuario);
        Assert.All(escenario.Sesiones.Items, s => Assert.False(s.Activa));
    }
}
