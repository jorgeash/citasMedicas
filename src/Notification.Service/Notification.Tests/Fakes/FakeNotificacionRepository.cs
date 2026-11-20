using Notification.Core.Interfaces.Repositories;
using Notification.Domain.Entities;

namespace Notification.Tests.Fakes;

public class FakeNotificacionRepository : InMemoryRepository<Notificacion>, INotificacionRepository
{
    public FakeNotificacionRepository()
        : base(n => n.NotificacionID, (n, id) => n.NotificacionID = id)
    {
    }
}
