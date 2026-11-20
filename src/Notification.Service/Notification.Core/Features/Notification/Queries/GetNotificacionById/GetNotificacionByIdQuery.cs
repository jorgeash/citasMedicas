using MediatR;
using Notification.Core.DTOs.Notification;
using Notification.Core.Interfaces.Repositories;

namespace Notification.Core.Features.Notification.Queries.GetNotificacionById;

public class GetNotificacionByIdQuery : IRequest<NotificacionDto?>
{
    public int NotificacionID { get; set; }

    public GetNotificacionByIdQuery(int notificacionId)
    {
        NotificacionID = notificacionId;
    }
}

public class GetNotificacionByIdQueryHandler : IRequestHandler<GetNotificacionByIdQuery, NotificacionDto?>
{
    private readonly INotificacionRepository _repository;

    public GetNotificacionByIdQueryHandler(INotificacionRepository repository)
    {
        _repository = repository;
    }

    public async Task<NotificacionDto?> Handle(GetNotificacionByIdQuery request, CancellationToken cancellationToken)
    {
        var notificacion = await _repository.GetByIdAsync(request.NotificacionID);
        return notificacion is null ? null : NotificationMapper.ToDto(notificacion);
    }
}
