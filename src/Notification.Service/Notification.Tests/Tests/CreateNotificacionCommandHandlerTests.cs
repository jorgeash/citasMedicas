using Notification.Core.Exceptions;
using Notification.Core.Features.Notification.Commands.CreateNotificacion;
using Notification.Tests.Fakes;

namespace Notification.Tests.Tests;

public class CreateNotificacionCommandHandlerTests
{
    [Fact]
    public async Task Handle_CreacionValida_GuardaComoPendiente()
    {
        var repository = new FakeNotificacionRepository();
        var handler = new CreateNotificacionCommandHandler(repository);
        var fechaProgramada = DateTime.UtcNow.AddDays(1);

        var result = await handler.Handle(new CreateNotificacionCommand
        {
            EventoID = " evt-1 ",
            UsuarioID = 1,
            Titulo = " Cita confirmada ",
            Mensaje = " Su cita fue confirmada. ",
            Tipo = " cita ",
            Canal = " interno ",
            Prioridad = " alta ",
            OrigenServicio = " scheduling ",
            Referencia = " ref-1 ",
            FechaProgramada = fechaProgramada
        }, CancellationToken.None);

        Assert.Equal("EVT-1", result.EventoID);
        Assert.Equal("CITA", result.Tipo);
        Assert.Equal("INTERNO", result.Canal);
        Assert.Equal("ALTA", result.Prioridad);
        Assert.Equal("SCHEDULING", result.OrigenServicio);
        Assert.Equal("Cita confirmada", result.Titulo);
        Assert.Equal("Su cita fue confirmada.", result.Mensaje);
        Assert.Equal("ref-1", result.Referencia);
        Assert.Equal("PENDIENTE", result.Estado);
        Assert.True(result.Activo);
        Assert.Equal(0, result.Intentos);
        Assert.Equal(fechaProgramada, result.FechaProgramada!.Value);
        Assert.Single(await repository.GetAllAsync());
    }

    [Fact]
    public async Task Handle_EventoDuplicado_DevuelveNotificacionExistenteSinInsertar()
    {
        var repository = new FakeNotificacionRepository();
        repository.Seed(NotificacionFactory.Crear(
            notificacionID: 7,
            eventoID: "EVT-1",
            estado: "ENVIADA"));

        var handler = new CreateNotificacionCommandHandler(repository);

        var result = await handler.Handle(ComandoValido(), CancellationToken.None);

        Assert.Equal(7, result.NotificacionID);
        Assert.Equal("ENVIADA", result.Estado);
        Assert.Single(await repository.GetAllAsync());
    }

    [Fact]
    public async Task Handle_TipoInvalido_LanzaDomainException()
    {
        var handler = new CreateNotificacionCommandHandler(new FakeNotificacionRepository());
        var command = ComandoValido();
        command.Tipo = "DESCONOCIDO";

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(command, CancellationToken.None));

        Assert.Equal(Errores.TIPO_INVALIDO, ex.Codigo);
    }

    [Fact]
    public async Task Handle_CanalInvalido_LanzaDomainException()
    {
        var handler = new CreateNotificacionCommandHandler(new FakeNotificacionRepository());
        var command = ComandoValido();
        command.Canal = "FAX";

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(command, CancellationToken.None));

        Assert.Equal(Errores.CANAL_INVALIDO, ex.Codigo);
    }

    [Fact]
    public async Task Handle_EmailSinArroba_LanzaDomainException()
    {
        var handler = new CreateNotificacionCommandHandler(new FakeNotificacionRepository());
        var command = ComandoValido();
        command.Canal = "EMAIL";
        command.Destinatario = "correo-sin-arroba";

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(command, CancellationToken.None));

        Assert.Equal(Errores.DESTINATARIO_INVALIDO, ex.Codigo);
    }

    [Fact]
    public async Task Handle_PrioridadInvalida_LanzaDomainException()
    {
        var handler = new CreateNotificacionCommandHandler(new FakeNotificacionRepository());
        var command = ComandoValido();
        command.Prioridad = "URGENTISIMA";

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(command, CancellationToken.None));

        Assert.Equal(Errores.PRIORIDAD_INVALIDA, ex.Codigo);
    }

    [Fact]
    public async Task Handle_OrigenInvalido_LanzaDomainException()
    {
        var handler = new CreateNotificacionCommandHandler(new FakeNotificacionRepository());
        var command = ComandoValido();
        command.OrigenServicio = "FARMACIA";

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(command, CancellationToken.None));

        Assert.Equal(Errores.ORIGEN_INVALIDO, ex.Codigo);
    }

    [Fact]
    public async Task Handle_FechaProgramadaPasada_LanzaDomainException()
    {
        var handler = new CreateNotificacionCommandHandler(new FakeNotificacionRepository());
        var command = ComandoValido();
        command.FechaProgramada = DateTime.UtcNow.AddDays(-1);

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(command, CancellationToken.None));

        Assert.Equal(Errores.FECHA_PROGRAMADA_INVALIDA, ex.Codigo);
    }

    private static CreateNotificacionCommand ComandoValido()
    {
        return new CreateNotificacionCommand
        {
            EventoID = "EVT-1",
            UsuarioID = 1,
            Titulo = "Cita confirmada",
            Mensaje = "Su cita fue confirmada.",
            Tipo = "CITA",
            Canal = "INTERNO",
            Prioridad = "NORMAL",
            OrigenServicio = "SCHEDULING"
        };
    }
}
