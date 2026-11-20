using Lab.Core.Exceptions;
using Lab.Core.Features.Lab.SolicitudesEstudio.Commands.CancelarSolicitudEstudio;
using Lab.Domain.Entities;
using Lab.Tests.Fakes;

namespace Lab.Tests.Tests;

public class CancelarSolicitudEstudioCommandHandlerTests
{
    private static (CancelarSolicitudEstudioCommandHandler handler, SolicitudEstudio solicitud) BuildHandler(
        string estado = "PENDIENTE",
        bool conResultado = false)
    {
        var repository = new FakeSolicitudEstudioRepository();
        var solicitud = new SolicitudEstudio
        {
            SolicitudEstudioID = 1,
            NumeroSolicitud = "SE-2026-000001",
            PacienteID = 10,
            Estado = estado,
            Observaciones = "Solicitud de prueba"
        };

        if (conResultado)
        {
            solicitud.Resultados.Add(new ResultadoEstudio
            {
                ResultadoEstudioID = 1,
                SolicitudEstudioID = 1,
                SolicitudEstudioDetalleID = 11,
                Valor = "7.2",
                Estado = "NORMAL"
            });
        }

        repository.Seed(solicitud);
        return (new CancelarSolicitudEstudioCommandHandler(repository), solicitud);
    }

    [Fact]
    public async Task Handle_ConResultados_LanzaDomainException()
    {
        var (handler, _) = BuildHandler(conResultado: true);

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(new CancelarSolicitudEstudioCommand
        {
            SolicitudEstudioID = 1,
            Motivo = "Error en la solicitud"
        }, CancellationToken.None));

        Assert.Equal(Errores.SOLICITUD_CON_RESULTADOS, ex.Codigo);
    }

    [Fact]
    public async Task Handle_SolicitudCompletada_LanzaDomainException()
    {
        var (handler, _) = BuildHandler(estado: "COMPLETADA");

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(new CancelarSolicitudEstudioCommand
        {
            SolicitudEstudioID = 1,
            Motivo = "Ya no procede"
        }, CancellationToken.None));

        Assert.Equal(Errores.SOLICITUD_ESTADO_INVALIDO, ex.Codigo);
    }

    [Fact]
    public async Task Handle_SinMotivo_LanzaDomainException()
    {
        var (handler, _) = BuildHandler();

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(new CancelarSolicitudEstudioCommand
        {
            SolicitudEstudioID = 1
        }, CancellationToken.None));

        Assert.Equal(Errores.MOTIVO_OBLIGATORIO, ex.Codigo);
    }

    [Fact]
    public async Task Handle_CancelacionValida_CambiaEstadoYGuardaMotivo()
    {
        var (handler, solicitud) = BuildHandler();

        var result = await handler.Handle(new CancelarSolicitudEstudioCommand
        {
            SolicitudEstudioID = 1,
            Motivo = "El médico la retiró"
        }, CancellationToken.None);

        Assert.Equal("CANCELADA", result.Estado);
        Assert.Contains("CANCELADA: El médico la retiró", result.Observaciones);
        Assert.NotNull(result.UpdatedAt);
        Assert.Equal("CANCELADA", solicitud.Estado);
    }
}
