using Identity.Core.DTOs.Identity;
using Identity.Core.Exceptions;
using Identity.Core.Features.Auth.Commands.Login;
using Identity.Core.Interfaces;
using Identity.Core.Interfaces.Repositories;
using MediatR;

namespace Identity.Core.Features.Auth.Commands.Refresh;

public class RefreshCommand : IRequest<AuthResultDto>
{
    public string RefreshToken { get; set; } = string.Empty;
}

public class RefreshCommandHandler : IRequestHandler<RefreshCommand, AuthResultDto>
{
    private readonly IUsuarioRepository _usuarios;
    private readonly ISesionRepository _sesiones;
    private readonly IPermisoRepository _permisos;
    private readonly ITokenService _tokens;

    public RefreshCommandHandler(
        IUsuarioRepository usuarios,
        ISesionRepository sesiones,
        IPermisoRepository permisos,
        ITokenService tokens)
    {
        _usuarios = usuarios;
        _sesiones = sesiones;
        _permisos = permisos;
        _tokens = tokens;
    }

    public async Task<AuthResultDto> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            throw new DomainException(Errores.REFRESH_INVALIDO, "El token de refresco es obligatorio.");

        var identidad = _tokens.ValidateRefreshToken(request.RefreshToken);
        if (identidad is null)
            throw new DomainException(Errores.REFRESH_INVALIDO, "El token de refresco no es válido o ha expirado.");

        var usuario = await _usuarios.GetCompletoPorIdAsync(identidad.UsuarioID, cancellationToken);
        if (usuario is null)
            throw new DomainException(Errores.REFRESH_INVALIDO, "El token de refresco no es válido o ha expirado.");

        if (!usuario.Activo)
            throw new DomainException(Errores.USUARIO_INACTIVO, "El usuario está inactivo. Contacte al administrador.");

        var permisos = await _permisos.GetCodigosPorRolAsync(usuario.RolID, cancellationToken);

        var (accessToken, expiresIn) = _tokens.CreateAccessToken(usuario, permisos);
        var refreshToken = _tokens.CreateRefreshToken(usuario);

        // Bitácora: cada canje deja constancia. La validación no lee esta tabla.
        await _sesiones.AbrirAsync(
            usuario.UsuarioID,
            tokenSesion: refreshToken,
            fechaInicio: DateTime.UtcNow,
            direccionIP: null,
            dispositivo: "refresh",
            cancellationToken);

        return new AuthResultDto
        {
            AccessToken = accessToken,
            ExpiresIn = expiresIn,
            RefreshToken = refreshToken,
            Usuario = LoginCommandHandler.Mapear(usuario, permisos)
        };
    }
}
