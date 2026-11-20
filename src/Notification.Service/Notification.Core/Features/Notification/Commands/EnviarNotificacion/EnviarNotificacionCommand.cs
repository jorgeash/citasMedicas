using MediatR;
using Notification.Core.DTOs.Notification;
using Notification.Core.Exceptions;
using Notification.Core.Interfaces.Repositories;

namespace Notification.Core.Features.Notification.Commands.EnviarNotificacion;

public class EnviarNotificacionCommand : IRequest<NotificacionDto>
{
    public int NotificacionID { get; set; }
    public bool Exito { get; set; } = true;
    public string? Error { get; set; }
}

public class EnviarNotificacionCommandHandler : IRequestHandler<EnviarNotificacionCommand, NotificacionDto>
{
    private readonly INotificacionRepository _repository;

    public EnviarNotificacionCommandHandler(INotificacionRepository repository)
    {
        _repository = repository;
    }

    public async Task<NotificacionDto> Handle(EnviarNotificacionCommand request, CancellationToken cancellationToken)
    {
        var notificacion = await _repository.GetByIdAsync(request.NotificacionID);
        if (notificacion is null)
            throw new DomainException(
                Errores.NOTIFICACION_NO_ENCONTRADA,
                $"No existe la notificación {request.NotificacionID}.");

        if (notificacion.Estado != Estados.Pendiente && notificacion.Estado != Estados.Fallida)
            throw new DomainException(
                Errores.ESTADO_NO_ENVIABLE,
                $"La notificación {request.NotificacionID} no se encuentra en un estado enviable.");

        string? error = null;
        if (!request.Exito)
        {
            error = NotificationMapper.Truncar(request.Error, 500);
            if (string.IsNullOrWhiteSpace(error))
                throw new DomainException(Errores.ERROR_ENVIO_OBLIGATORIO, "El error del envío es obligatorio cuando Exito es false.");
        }

        var ahora = DateTime.UtcNow;
        notificacion.Intentos += 1;
        notificacion.UpdatedAt = ahora;

        if (request.Exito)
        {
            notificacion.Estado = Estados.Enviada;
            notificacion.FechaEnvio = ahora;
            notificacion.UltimoError = null;
        }
        else
        {
            notificacion.Estado = Estados.Fallida;
            notificacion.UltimoError = error;
        }

        await _repository.UpdateAsync(notificacion);
        return NotificationMapper.ToDto(notificacion);
    }
}
