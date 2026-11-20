using Notification.Core.Exceptions;
using Notification.Core.Features.Notification.Commands.EnviarNotificacion;
using Notification.Tests.Fakes;

namespace Notification.Tests.Tests;

public class EnviarNotificacionCommandHandlerTests
{
    [Fact]
    public async Task Handle_EnvioExitoso_ActualizaEstadoFechaEIntentos()
    {
        var repository = new FakeNotificacionRepository();
        repository.Seed(NotificacionFactory.Crear(estado: "PENDIENTE"));
        var handler = new EnviarNotificacionCommandHandler(repository);

        var result = await handler.Handle(new EnviarNotificacionCommand
        {
            NotificacionID = 1,
            Exito = true
        }, CancellationToken.None);

        Assert.Equal("ENVIADA", result.Estado);
        Assert.NotNull(result.FechaEnvio);
        Assert.NotNull(result.UpdatedAt);
        Assert.Null(result.UltimoError);
        Assert.Equal(1, result.Intentos);
    }

    [Fact]
    public async Task Handle_EnvioFallidoConError_GuardaUltimoErrorEIntentos()
    {
        var repository = new FakeNotificacionRepository();
        repository.Seed(NotificacionFactory.Crear(estado: "PENDIENTE"));
        var handler = new EnviarNotificacionCommandHandler(repository);

        var result = await handler.Handle(new EnviarNotificacionCommand
        {
            NotificacionID = 1,
            Exito = false,
            Error = "  Servidor SMTP no disponible  "
        }, CancellationToken.None);

        Assert.Equal("FALLIDA", result.Estado);
        Assert.Equal("Servidor SMTP no disponible", result.UltimoError);
        Assert.Null(result.FechaEnvio);
        Assert.NotNull(result.UpdatedAt);
        Assert.Equal(1, result.Intentos);
    }

    [Fact]
    public async Task Handle_EnvioFallidoSinError_LanzaDomainException()
    {
        var repository = new FakeNotificacionRepository();
        repository.Seed(NotificacionFactory.Crear(estado: "PENDIENTE"));
        var handler = new EnviarNotificacionCommandHandler(repository);

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(new EnviarNotificacionCommand
        {
            NotificacionID = 1,
            Exito = false,
            Error = "   "
        }, CancellationToken.None));

        Assert.Equal(Errores.ERROR_ENVIO_OBLIGATORIO, ex.Codigo);
    }

    [Fact]
    public async Task Handle_EstadoNoEnviable_LanzaDomainException()
    {
        var repository = new FakeNotificacionRepository();
        repository.Seed(NotificacionFactory.Crear(estado: "ENVIADA"));
        var handler = new EnviarNotificacionCommandHandler(repository);

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(new EnviarNotificacionCommand
        {
            NotificacionID = 1,
            Exito = true
        }, CancellationToken.None));

        Assert.Equal(Errores.ESTADO_NO_ENVIABLE, ex.Codigo);
    }
}
