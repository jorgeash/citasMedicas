using Identity.Core.DTOs.Identity;
using Identity.Core.Exceptions;
using Identity.Core.Interfaces;
using Identity.Core.Interfaces.Repositories;
using Identity.Domain.Entities;
using MediatR;

namespace Identity.Core.Features.Auth.Commands.Login;

public class LoginCommand : IRequest<AuthResultDto>
{
    public string NombreUsuario { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    /// <summary>Lo completa el controller con la IP real de la petición.</summary>
    public string? DireccionIP { get; set; }

    /// <summary>Lo completa el controller con el header User-Agent.</summary>
    public string? Dispositivo { get; set; }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResultDto>
{
    private readonly IUsuarioRepository _usuarios;
    private readonly ISesionRepository _sesiones;
    private readonly IPermisoRepository _permisos;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenService _tokens;

    public LoginCommandHandler(
        IUsuarioRepository usuarios,
        ISesionRepository sesiones,
        IPermisoRepository permisos,
        IPasswordHasher hasher,
        ITokenService tokens)
    {
        _usuarios = usuarios;
        _sesiones = sesiones;
        _permisos = permisos;
        _hasher = hasher;
        _tokens = tokens;
    }

    public async Task<AuthResultDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.NombreUsuario) || string.IsNullOrWhiteSpace(request.Password))
            throw new DomainException(Errores.DATOS_INVALIDOS, "El usuario y la contraseña son obligatorios.");


        var usuario = await _usuarios.GetCompletoPorNombreAsync(request.NombreUsuario.Trim(), cancellationToken);

        // Mismo mensaje existente o no: no se revela qué cuentas hay en el sistema.
        if (usuario is null || !_hasher.Verify(request.Password, usuario.PasswordHash))
            throw new DomainException(Errores.CREDENCIALES_INVALIDAS, "Usuario o contraseña incorrectos.");

        if (!usuario.Activo)
            throw new DomainException(Errores.USUARIO_INACTIVO, "El usuario está inactivo. Contacte al administrador.");

        var permisos = await _permisos.GetCodigosPorRolAsync(usuario.RolID, cancellationToken);

        var (accessToken, expiresIn) = _tokens.CreateAccessToken(usuario, permisos);
        var refreshToken = _tokens.CreateRefreshToken(usuario);

        var ahora = DateTime.UtcNow;
        await _sesiones.AbrirAsync(
            usuario.UsuarioID,
            tokenSesion: refreshToken,
            fechaInicio: ahora,
            direccionIP: request.DireccionIP,
            dispositivo: request.Dispositivo,
            cancellationToken);

        await _usuarios.RegistrarAccesoAsync(usuario.UsuarioID, ahora, cancellationToken);

        return new AuthResultDto
        {
            AccessToken = accessToken,
            ExpiresIn = expiresIn,
            RefreshToken = refreshToken,
            Usuario = Mapear(usuario, permisos)
        };
    }

    internal static UsuarioSesionDto Mapear(Usuario usuario, IReadOnlyCollection<string> permisos)
    {
        return new UsuarioSesionDto
        {
            UsuarioID = usuario.UsuarioID,
            NombreUsuario = usuario.NombreUsuario,
            NombreCompleto = usuario.NombreCompleto,
            Email = usuario.Email,
            Rol = usuario.Rol?.Nombre ?? string.Empty,
            SucursalID = usuario.SucursalID,
            Permisos = permisos.OrderBy(p => p, StringComparer.Ordinal).ToList()
        };
    }
}
