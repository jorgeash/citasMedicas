using MediatR;
using Notification.Core.DTOs.Notification;
using Notification.Core.Exceptions;
using Notification.Core.Interfaces.Repositories;

namespace Notification.Core.Features.Notification.Commands.CancelarNotificacion;

public class CancelarNotificacionCommand : IRequest<NotificacionDto>
{
    public int NotificacionID { get; set; }
    public string? Motivo { get; set; }
}

public class CancelarNotificacionCommandHandler : IRequestHandler<CancelarNotificacionCommand, NotificacionDto>
{
    private readonly INotificacionRepository _repository;

    public CancelarNotificacionCommandHandler(INotificacionRepository repository)
    {
        _repository = repository;
    }

    public async Task<NotificacionDto> Handle(CancelarNotificacionCommand request, CancellationToken cancellationToken)
    {
        var notificacion = await _repository.GetByIdAsync(request.NotificacionID);
        if (notificacion is null)
            throw new DomainException(
                Errores.NOTIFICACION_NO_ENCONTRADA,
                $"No existe la notificación {request.NotificacionID}.");

        if (notificacion.Estado == Estados.Leida || notificacion.Estado == Estados.Cancelada)
            throw new DomainException(
                Errores.ESTADO_NO_CANCELABLE,
                $"La notificación {request.NotificacionID} no se encuentra en un estado cancelable.");

        var motivo = NotificationMapper.Truncar(request.Motivo, 300);
        if (string.IsNullOrWhiteSpace(motivo))
            throw new DomainException(Errores.MOTIVO_OBLIGATORIO, "El motivo de la cancelación es obligatorio.");

        var ahora = DateTime.UtcNow;
        notificacion.Estado = Estados.Cancelada;
        notificacion.FechaCancelacion = ahora;
        notificacion.MotivoCancelacion = motivo;
        notificacion.Activo = false;
        notificacion.UpdatedAt = ahora;

        await _repository.UpdateAsync(notificacion);
        return NotificationMapper.ToDto(notificacion);
    }
}
