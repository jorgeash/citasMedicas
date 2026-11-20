using MediatR;
using Notification.Core.DTOs.Notification;
using Notification.Core.Exceptions;
using Notification.Core.Interfaces.Repositories;

namespace Notification.Core.Features.Notification.Commands.MarcarNotificacionLeida;

public class MarcarNotificacionLeidaCommand : IRequest<NotificacionDto>
{
    public int NotificacionID { get; set; }
}

public class MarcarNotificacionLeidaCommandHandler : IRequestHandler<MarcarNotificacionLeidaCommand, NotificacionDto>
{
    private readonly INotificacionRepository _repository;

    public MarcarNotificacionLeidaCommandHandler(INotificacionRepository repository)
    {
        _repository = repository;
    }

    public async Task<NotificacionDto> Handle(MarcarNotificacionLeidaCommand request, CancellationToken cancellationToken)
    {
        var notificacion = await _repository.GetByIdAsync(request.NotificacionID);
        if (notificacion is null)
            throw new DomainException(
                Errores.NOTIFICACION_NO_ENCONTRADA,
                $"No existe la notificación {request.NotificacionID}.");

        if (notificacion.Estado == Estados.Cancelada)
            throw new DomainException(
                Errores.ESTADO_NO_LEIBLE,
                $"La notificación {request.NotificacionID} está cancelada y no puede marcarse como leída.");

        if (notificacion.Estado == Estados.Leida)
            return NotificationMapper.ToDto(notificacion);

        var ahora = DateTime.UtcNow;
        notificacion.Estado = Estados.Leida;
        notificacion.FechaLectura = ahora;
        notificacion.UpdatedAt = ahora;

        await _repository.UpdateAsync(notificacion);
        return NotificationMapper.ToDto(notificacion);
    }
}
