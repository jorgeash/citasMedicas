using Notifications.Core.DTOs.External;
using Notifications.Core.Exceptions;
using Notifications.Core.Features.Notificaciones.Commands.EnviarConfirmacionPago;
using Notifications.Core.Interfaces.Services;
using Notifications.Domain.Entities;
using NugetPackage_Rest.Exceptions;

namespace Notifications.Tests.Features;

/// <summary>
/// El handler se prueba con dobles de las interfaces de Core: así se verifica la lógica de
/// negocio sin tocar HTTP, y queda demostrado que el handler no depende de IRest.
/// </summary>
public class EnviarConfirmacionPagoCommandHandlerTests
{
    private const int FacturaId = 42;

    private static FacturaPagadaResponse Factura(string estado = "PAGADA", string email = "ana@clinica.test") => new()
    {
        FacturaId = FacturaId,
        Paciente = "Ana Ruiz",
        Email = email,
        Monto = 120.50m,
        Moneda = "USD",
        Estado = estado,
        TransaccionId = "TX-9"
    };

    private sealed class FacturaServiceStub : IFacturaService
    {
        private readonly Func<int, FacturaPagadaResponse>? _respuesta;
        private readonly Exception? _error;

        public FacturaPagadaResponse? Consultada { get; private set; }

        public FacturaServiceStub(FacturaPagadaResponse respuesta)
        {
            _respuesta = _ => respuesta;
        }

        public FacturaServiceStub(Exception error)
        {
            _error = error;
        }

        public Task<FacturaPagadaResponse> ConsultarFacturaPagadaAsync(int facturaId, CancellationToken cancellationToken = default)
        {
            if (_error != null) throw _error;

            var factura = _respuesta!(facturaId);
            Consultada = factura;
            return Task.FromResult(factura);
        }
    }

    private sealed class EmailSenderStub : IEmailSender
    {
        public int Llamadas { get; private set; }
        public string? Destinatario { get; private set; }
        public string? Cuerpo { get; private set; }

        public Task<NotificacionEnviada> EnviarAsync(int facturaId, string destinatario, string asunto, string cuerpo, CancellationToken cancellationToken = default)
        {
            Llamadas++;
            Destinatario = destinatario;
            Cuerpo = cuerpo;

            return Task.FromResult(new NotificacionEnviada
            {
                NotificacionId = 1,
                FacturaId = facturaId,
                Destinatario = destinatario,
                Asunto = asunto,
                MessageId = "<sim-1@notificaciones.local>",
                EnviadaUtc = DateTime.UtcNow
            });
        }
    }

    [Fact]
    public async Task Handle_FacturaPagada_ConsultaBillingYEnviaElCorreoAlPaciente()
    {
        var facturas = new FacturaServiceStub(Factura());
        var correos = new EmailSenderStub();
        var handler = new EnviarConfirmacionPagoCommandHandler(facturas, correos);

        var response = await handler.Handle(new EnviarConfirmacionPagoCommand { FacturaId = FacturaId }, CancellationToken.None);

        Assert.True(response.Succeeded);
        Assert.Equal(FacturaId, response.Data.FacturaId);
        Assert.Equal("ana@clinica.test", correos.Destinatario);
        Assert.Contains("120.50", correos.Cuerpo);
        Assert.Contains("Ana Ruiz", correos.Cuerpo);
        Assert.Equal(1, correos.Llamadas);
    }

    [Fact]
    public async Task Handle_FacturaNoPagada_LanzaDomainExceptionYNoEnviaCorreo()
    {
        var correos = new EmailSenderStub();
        var handler = new EnviarConfirmacionPagoCommandHandler(
            new FacturaServiceStub(Factura(estado: "PENDIENTE")), correos);

        var exception = await Assert.ThrowsAsync<DomainException>(
            () => handler.Handle(new EnviarConfirmacionPagoCommand { FacturaId = FacturaId }, CancellationToken.None));

        Assert.Equal(Errores.FACTURA_NO_PAGADA, exception.Codigo);
        Assert.Equal(0, correos.Llamadas);
    }

    [Fact]
    public async Task Handle_SinCorreoRegistrado_LanzaDomainExceptionYNoEnviaCorreo()
    {
        var correos = new EmailSenderStub();
        var handler = new EnviarConfirmacionPagoCommandHandler(
            new FacturaServiceStub(Factura(email: "")), correos);

        var exception = await Assert.ThrowsAsync<DomainException>(
            () => handler.Handle(new EnviarConfirmacionPagoCommand { FacturaId = FacturaId }, CancellationToken.None));

        Assert.Equal(Errores.CORREO_NO_ENVIADO, exception.Codigo);
        Assert.Equal(0, correos.Llamadas);
    }

    [Fact]
    public async Task Handle_FallaTecnicaDeBilling_TraduceApiExceptionADomainExceptionDeServicioNoDisponible()
    {
        var correos = new EmailSenderStub();
        var handler = new EnviarConfirmacionPagoCommandHandler(
            new FacturaServiceStub(new ApiException("timeout", ApiFailureReason.Timeout)), correos);

        var exception = await Assert.ThrowsAsync<DomainException>(
            () => handler.Handle(new EnviarConfirmacionPagoCommand { FacturaId = FacturaId }, CancellationToken.None));

        Assert.Equal(Errores.SERVICIO_NO_DISPONIBLE, exception.Codigo);
        Assert.Equal(0, correos.Llamadas);
    }

    [Fact]
    public async Task Handle_DomainExceptionDeBilling_NoSeTraduceNiSeReenvuelve()
    {
        var handler = new EnviarConfirmacionPagoCommandHandler(
            new FacturaServiceStub(new DomainException(Errores.FACTURA_NO_ENCONTRADA, "no existe")),
            new EmailSenderStub());

        var exception = await Assert.ThrowsAsync<DomainException>(
            () => handler.Handle(new EnviarConfirmacionPagoCommand { FacturaId = FacturaId }, CancellationToken.None));

        Assert.Equal(Errores.FACTURA_NO_ENCONTRADA, exception.Codigo);
    }
}
