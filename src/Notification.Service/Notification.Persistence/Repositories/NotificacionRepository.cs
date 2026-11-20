using Notification.Core.Interfaces.Repositories;
using Notification.Domain.Entities;
using Notification.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Notification.Persistence.Repositories;

public class NotificacionRepository : Repository<Notificacion>, INotificacionRepository
{
    public NotificacionRepository(NotificationDbContext context) : base(context) { }
}
