using Notification.Core.Features.Notification.Queries.GetNotificaciones;
using Notification.Tests.Fakes;

namespace Notification.Tests.Tests;

public class GetNotificacionesQueryHandlerTests
{
    [Fact]
    public async Task Handle_FiltrosPaginacionBusquedaYExpresion_DevuelveResultadosEsperados()
    {
        var repository = new FakeNotificacionRepository();
        repository.Seed(
            NotificacionFactory.Crear(
                notificacionID: 1,
                eventoID: "EVT-1",
                usuarioID: 1,
                estado: "PENDIENTE",
                tipo: "CITA",
                canal: "INTERNO",
                origenServicio: "SCHEDULING",
                titulo: "Confirmación de cita",
                mensaje: "Cita en consultorio",
                referencia: "REF-1",
                fechaCreacion: new DateTime(2026, 1, 10, 8, 0, 0, DateTimeKind.Utc)),
            NotificacionFactory.Crear(
                notificacionID: 2,
                eventoID: "EVT-2",
                usuarioID: 1,
                estado: "ENVIADA",
                tipo: "RECORDATORIO",
                canal: "EMAIL",
                origenServicio: "SCHEDULING",
                destinatario: "paciente@correo.com",
                titulo: "Recordatorio de cita",
                mensaje: "No olvide su cita",
                referencia: "REF-2",
                fechaCreacion: new DateTime(2026, 1, 15, 9, 0, 0, DateTimeKind.Utc)),
            NotificacionFactory.Crear(
                notificacionID: 3,
                eventoID: "EVT-3",
                usuarioID: 2,
                estado: "ENVIADA",
                tipo: "RESULTADO",
                canal: "EMAIL",
                origenServicio: "LABORATORY",
                destinatario: "otro@correo.com",
                titulo: "Resultado de laboratorio",
                mensaje: "Su resultado está disponible",
                referencia: "REF-3",
                activo: false,
                fechaCreacion: new DateTime(2026, 2, 1, 10, 0, 0, DateTimeKind.Utc)),
            NotificacionFactory.Crear(
                notificacionID: 4,
                eventoID: "EVT-4",
                usuarioID: 1,
                estado: "CANCELADA",
                tipo: "CITA",
                canal: "INTERNO",
                origenServicio: "SCHEDULING",
                titulo: "Cita cancelada",
                mensaje: "La cita fue cancelada",
                referencia: "REF-4",
                fechaCreacion: new DateTime(2026, 2, 10, 11, 0, 0, DateTimeKind.Utc)));

        var handler = new GetNotificacionesQueryHandler(repository);

        var filtrados = await handler.Handle(new GetNotificacionesQuery
        {
            UsuarioID = 1,
            Tipo = "cita",
            Estado = "pendiente",
            SoloActivas = true,
            Desde = new DateTime(2026, 1, 1),
            Hasta = new DateTime(2026, 1, 31)
        }, CancellationToken.None);

        Assert.Equal(1, filtrados.TotalRecords);
        Assert.Equal(1, Assert.Single(filtrados.Data).NotificacionID);

        var porBusqueda = await handler.Handle(new GetNotificacionesQuery
        {
            Buscar = "REF-3"
        }, CancellationToken.None);

        Assert.Equal(3, Assert.Single(porBusqueda.Data).NotificacionID);

        var porExpresion = await handler.Handle(new GetNotificacionesQuery
        {
            Filter = "x.Canal == \"EMAIL\"",
            OrderBy = "NotificacionID desc",
            PageNumber = 1,
            PageSize = 2
        }, CancellationToken.None);

        Assert.Equal(2, porExpresion.TotalRecords);
        Assert.Equal(2, porExpresion.PageSize);
        Assert.Equal(3, porExpresion.Data[0].NotificacionID);
        Assert.Equal(2, porExpresion.Data[1].NotificacionID);

        var paginaDos = await handler.Handle(new GetNotificacionesQuery
        {
            OrderBy = "NotificacionID asc",
            PageNumber = 2,
            PageSize = 2
        }, CancellationToken.None);

        Assert.Equal(4, paginaDos.TotalRecords);
        Assert.Equal(2, paginaDos.TotalPage);
        Assert.Equal(2, paginaDos.CurrentPage);
        Assert.Equal(2, paginaDos.Data.Count);
        Assert.Equal(3, paginaDos.Data[0].NotificacionID);
        Assert.Equal(4, paginaDos.Data[1].NotificacionID);
        Assert.Equal(100, new GetNotificacionesQuery { PageSize = 500 }.PageSize);
    }
}
