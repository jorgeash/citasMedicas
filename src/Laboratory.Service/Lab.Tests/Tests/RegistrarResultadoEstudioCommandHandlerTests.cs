using Lab.Core.Exceptions;
using Lab.Core.Features.Lab.ResultadosEstudio.Commands.RegistrarResultadoEstudio;
using Lab.Domain.Entities;
using Lab.Tests.Fakes;

namespace Lab.Tests.Tests;

public class RegistrarResultadoEstudioCommandHandlerTests
{
    private static RegistrarResultadoEstudioCommand NuevoCommand(int detalleID = 12) => new()
    {
        SolicitudEstudioID = 1,
        SolicitudEstudioDetalleID = detalleID,
        Valor = "7.2",
        Unidad = "g/dL",
        Estado = "NORMAL"
    };

    private static (
        RegistrarResultadoEstudioCommandHandler handler,
        SolicitudEstudio solicitud,
        FakeResultadoEstudioRepository resultados) BuildHandler(
            string estado = "PENDIENTE",
            bool conResultado = false)
    {
        var solicitudes = new FakeSolicitudEstudioRepository();
        var resultados = new FakeResultadoEstudioRepository();

        var solicitud = new SolicitudEstudio
        {
            SolicitudEstudioID = 1,
            NumeroSolicitud = "SE-2026-000001",
            AtencionID = 10,
            PacienteID = 10,
            MedicoID = 5,
            Estado = estado,
            FechaSolicitud = DateTime.UtcNow
        };

        solicitud.Detalle.Add(new SolicitudEstudioDetalle
        {
            SolicitudEstudioDetalleID = 11,
            SolicitudEstudioID = 1,
            EstudioID = 1
        });
        solicitud.Detalle.Add(new SolicitudEstudioDetalle
        {
            SolicitudEstudioDetalleID = 12,
            SolicitudEstudioID = 1,
            EstudioID = 2
        });

        if (conResultado)
        {
            var previo = new ResultadoEstudio
            {
                ResultadoEstudioID = 100,
                SolicitudEstudioID = 1,
                SolicitudEstudioDetalleID = 11,
                Valor = "5.1",
                Estado = "NORMAL",
                SolicitudEstudio = solicitud
            };
            solicitud.Resultados.Add(previo);
            resultados.Seed(previo);
        }

        solicitudes.Seed(solicitud);

        var handler = new RegistrarResultadoEstudioCommandHandler(resultados, solicitudes);
        return (handler, solicitud, resultados);
    }

    [Fact]
    public async Task Handle_PrimerResultado_PasaAEnProcesoPeroNoCompleta()
    {
        var (handler, solicitud, resultados) = BuildHandler();

        var dto = await handler.Handle(NuevoCommand(11), CancellationToken.None);

        Assert.Equal("EN_PROCESO", solicitud.Estado);
        Assert.Null(solicitud.UpdatedAt);
        Assert.Equal("SE-2026-000001", dto.NumeroSolicitud);
        Assert.Single(await resultados.GetAllAsync());
    }

    [Fact]
    public async Task Handle_UltimoResultado_CompletaSolicitudYActualizaFecha()
    {
        var (handler, solicitud, _) = BuildHandler(conResultado: true);

        await handler.Handle(NuevoCommand(12), CancellationToken.None);

        Assert.Equal("COMPLETADA", solicitud.Estado);
        Assert.NotNull(solicitud.UpdatedAt);
    }

    [Fact]
    public async Task Handle_ResultadoDuplicado_LanzaDomainException()
    {
        var (handler, _, _) = BuildHandler(conResultado: true);

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            handler.Handle(NuevoCommand(11), CancellationToken.None));

        Assert.Equal(Errores.RESULTADO_DUPLICADO, ex.Codigo);
    }

    [Fact]
    public async Task Handle_SolicitudCancelada_LanzaDomainException()
    {
        var (handler, _, _) = BuildHandler(estado: "CANCELADA");

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            handler.Handle(NuevoCommand(11), CancellationToken.None));

        Assert.Equal(Errores.SOLICITUD_CANCELADA, ex.Codigo);
    }

    [Fact]
    public async Task Handle_EstadoResultadoInvalido_LanzaDomainException()
    {
        var (handler, _, _) = BuildHandler();
        var command = NuevoCommand(11);
        command.Estado = "DESCONOCIDO";

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(command, CancellationToken.None));

        Assert.Equal(Errores.ESTADO_RESULTADO_INVALIDO, ex.Codigo);
    }

    [Fact]
    public async Task Handle_DetalleAjeno_LanzaDomainException()
    {
        var (handler, _, _) = BuildHandler();

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            handler.Handle(NuevoCommand(999), CancellationToken.None));

        Assert.Equal(Errores.DETALLE_NO_ENCONTRADO, ex.Codigo);
    }

    [Fact]
    public async Task Handle_ResultadoValidado_RegistraFechaDeValidacion()
    {
        var (handler, _, _) = BuildHandler();
        var command = NuevoCommand(11);
        command.ValidadoPorMedicoID = 5;

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(5, result.ValidadoPorMedicoID);
        Assert.NotNull(result.FechaValidacion);
    }
}
