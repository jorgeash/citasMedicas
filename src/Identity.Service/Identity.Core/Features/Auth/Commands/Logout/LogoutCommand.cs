using Identity.Core.Exceptions;
using Identity.Core.Interfaces.Repositories;
using MediatR;

namespace Identity.Core.Features.Auth.Commands.Logout;

public class LogoutCommand : IRequest
{
    public int UsuarioID { get; set; }
}

public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly ISesionRepository _sesiones;

    public LogoutCommandHandler(ISesionRepository sesiones)
    {
        _sesiones = sesiones;
    }

    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        if (request.UsuarioID <= 0)
            throw new DomainException(Errores.NO_AUTENTICADO, "Debe iniciar sesión para cerrarla.");

        // Stateless: el access token sigue siendo válido hasta que venza; aquí
        // solo se cierra la bitácora para que quede registrado el cierre.
        await _sesiones.CerrarTodasAsync(request.UsuarioID, DateTime.UtcNow, cancellationToken);
    }
}
