using Notification.Core.Exceptions;
using Notification.Core.Features.Notification.Commands.MarcarNotificacionLeida;
using Notification.Tests.Fakes;

namespace Notification.Tests.Tests;

public class MarcarNotificacionLeidaCommandHandlerTests
{
    [Fact]
    public async Task Handle_NotificacionPendiente_MarcaComoLeida()
    {
        var repository = new FakeNotificacionRepository();
        repository.Seed(NotificacionFactory.Crear(estado: "PENDIENTE"));
        var handler = new MarcarNotificacionLeidaCommandHandler(repository);

        var result = await handler.Handle(new MarcarNotificacionLeidaCommand
        {
            NotificacionID = 1
        }, CancellationToken.None);

        Assert.Equal("LEIDA", result.Estado);
        Assert.NotNull(result.FechaLectura);
        Assert.NotNull(result.UpdatedAt);
    }

    [Fact]
    public async Task Handle_NotificacionYaLeida_DevuelveIgualSinCambios()
    {
        var fechaLectura = new DateTime(2026, 3, 1, 10, 0, 0, DateTimeKind.Utc);
        var repository = new FakeNotificacionRepository();
        repository.Seed(NotificacionFactory.Crear(estado: "LEIDA", fechaLectura: fechaLectura));
        var handler = new MarcarNotificacionLeidaCommandHandler(repository);
        var command = new MarcarNotificacionLeidaCommand { NotificacionID = 1 };

        var primero = await handler.Handle(command, CancellationToken.None);
        var segundo = await handler.Handle(command, CancellationToken.None);

        Assert.Equal("LEIDA", segundo.Estado);
        Assert.Equal(fechaLectura, segundo.FechaLectura!.Value);
        Assert.Equal(primero.FechaLectura, segundo.FechaLectura);
    }

    [Fact]
    public async Task Handle_NotificacionCancelada_LanzaDomainException()
    {
        var repository = new FakeNotificacionRepository();
        repository.Seed(NotificacionFactory.Crear(estado: "CANCELADA", activo: false));
        var handler = new MarcarNotificacionLeidaCommandHandler(repository);

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(
            new MarcarNotificacionLeidaCommand { NotificacionID = 1 },
            CancellationToken.None));

        Assert.Equal(Errores.ESTADO_NO_LEIBLE, ex.Codigo);
    }
}
