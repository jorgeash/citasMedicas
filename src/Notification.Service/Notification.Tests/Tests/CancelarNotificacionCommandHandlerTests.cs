using Notification.Core.Exceptions;
using Notification.Core.Features.Notification.Commands.CancelarNotificacion;
using Notification.Tests.Fakes;

namespace Notification.Tests.Tests;

public class CancelarNotificacionCommandHandlerTests
{
    [Fact]
    public async Task Handle_CancelacionValida_DesactivaYRegistraMotivo()
    {
        var repository = new FakeNotificacionRepository();
        repository.Seed(NotificacionFactory.Crear(estado: "PENDIENTE"));
        var handler = new CancelarNotificacionCommandHandler(repository);

        var result = await handler.Handle(new CancelarNotificacionCommand
        {
            NotificacionID = 1,
            Motivo = "  El paciente reprogramó la cita  "
        }, CancellationToken.None);

        Assert.Equal("CANCELADA", result.Estado);
        Assert.False(result.Activo);
        Assert.NotNull(result.FechaCancelacion);
        Assert.NotNull(result.UpdatedAt);
        Assert.Equal("El paciente reprogramó la cita", result.MotivoCancelacion);
    }

    [Fact]
    public async Task Handle_CancelacionSinMotivo_LanzaDomainException()
    {
        var repository = new FakeNotificacionRepository();
        repository.Seed(NotificacionFactory.Crear(estado: "PENDIENTE"));
        var handler = new CancelarNotificacionCommandHandler(repository);

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(new CancelarNotificacionCommand
        {
            NotificacionID = 1,
            Motivo = "   "
        }, CancellationToken.None));

        Assert.Equal(Errores.MOTIVO_OBLIGATORIO, ex.Codigo);
    }
}
