using Identity.Core.DTOs.Identity;
using Identity.Core.Exceptions;
using Identity.Core.Features.Auth.Commands.Login;
using Identity.Core.Interfaces;
using Identity.Core.Interfaces.Repositories;
using MediatR;

namespace Identity.Core.Features.Auth.Queries.GetMe;

public class GetMeQuery : IRequest<UsuarioSesionDto>
{
    public int UsuarioID { get; set; }
}

public class GetMeQueryHandler : IRequestHandler<GetMeQuery, UsuarioSesionDto>
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IPermisoRepository _permisos;

    public GetMeQueryHandler(IUsuarioRepository usuarios, IPermisoRepository permisos)
    {
        _usuarios = usuarios;
        _permisos = permisos;
    }

    public async Task<UsuarioSesionDto> Handle(GetMeQuery request, CancellationToken cancellationToken)
    {
        if (request.UsuarioID <= 0)
            throw new DomainException(Errores.NO_AUTENTICADO, "Debe iniciar sesión para consultar su perfil.");

        var usuario = await _usuarios.GetCompletoPorIdAsync(request.UsuarioID, cancellationToken);
        if (usuario is null)
            throw new DomainException(Errores.NO_AUTENTICADO, "Debe iniciar sesión para consultar su perfil.");

        if (!usuario.Activo)
            throw new DomainException(Errores.USUARIO_INACTIVO, "El usuario está inactivo. Contacte al administrador.");

        var permisos = await _permisos.GetCodigosPorRolAsync(usuario.RolID, cancellationToken);

        return LoginCommandHandler.Mapear(usuario, permisos);
    }
}
