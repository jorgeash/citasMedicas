using Billing.Core.DTOs.External;
using Billing.Core.Exceptions;
using Billing.Core.Features.Billing.Cobros.Commands.ProcesarCobro;
using Billing.Core.Interfaces.Services;

namespace Billing.Tests.Features;

public class ProcesarCobroCommandHandlerTests
{
    private sealed class StubCitaService : ICitaService
    {
        private readonly bool _disponible;
        public StubCitaService(bool disponible) => _disponible = disponible;

        public Task<bool> EstaDisponibleAsync(int servicioId, DateTime fecha, CancellationToken cancellationToken = default)
            => Task.FromResult(_disponible);
    }

    private sealed class StubPagoService : IPagoService
    {
        public string Transaccion { get; set; } = "TX-INTERNA";

        public Task<string> ProcesarPagoAsync(int facturaId, decimal monto, string moneda, CancellationToken cancellationToken = default)
            => Task.FromResult(Transaccion);
    }

    private sealed class StubPasarelaGateway : IPasarelaGateway
    {
        private readonly CobroResponse? _cobro;
        private readonly Exception? _error;

        public StubPasarelaGateway(CobroResponse? cobro, Exception? error = null)
        {
            _cobro = cobro;
            _error = error;
        }

        public Task<CobroResponse> CobrarAsync(CobroRequest request, CancellationToken cancellationToken = default)
            => _error != null ? Task.FromException<CobroResponse>(_error) : Task.FromResult(_cobro!);

        public Task<CobroResponse> ConsultarCobroAsync(string referencia, CancellationToken cancellationToken = default)
            => Task.FromResult(_cobro!);
    }

    private static ProcesarCobroCommand Command() => new()
    {
        FacturaId = 77,
        ServicioId = 12,
        Monto = 25.50m,
        Moneda = "USD",
        FechaCita = new DateTime(2026, 3, 15)
    };

    [Fact]
    public async Task Handle_CobroAprobado_DevuelveLaTransaccionDeLaPasarela()
    {
        var gateway = new StubPasarelaGateway(new CobroResponse { Estado = "APROBADO", TransaccionId = "TX-9" });
        var handler = new ProcesarCobroCommandHandler(new StubCitaService(true), new StubPagoService(), gateway);

        var result = await handler.Handle(Command(), CancellationToken.None);

        Assert.Equal("TX-9", result.Data);
    }

    [Fact]
    public async Task Handle_SinDisponibilidad_LanzaDomainExceptionDeCita()
    {
        var handler = new ProcesarCobroCommandHandler(
            new StubCitaService(false),
            new StubPagoService(),
            new StubPasarelaGateway(new CobroResponse { Estado = "APROBADO" }));

        var exception = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(Command(), CancellationToken.None));
        Assert.Equal(Errores.CITA_NO_DISPONIBLE, exception.Codigo);
    }

    [Fact]
    public async Task Handle_CobroRechazado_LanzaDomainExceptionDePago()
    {
        var gateway = new StubPasarelaGateway(new CobroResponse
        {
            Estado = "RECHAZADO",
            Mensajes = new List<string> { "Tarjeta rechazada" }
        });
        var handler = new ProcesarCobroCommandHandler(new StubCitaService(true), new StubPagoService(), gateway);

        var exception = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(Command(), CancellationToken.None));
        Assert.Equal(Errores.PAGO_FALLIDO, exception.Codigo);
        Assert.Contains("Tarjeta rechazada", exception.Message);
    }

    [Fact]
    public async Task Handle_PasarelaCaida_RecurreAlCobroInternoDePagos()
    {
        var pagoService = new StubPagoService();
        // Tras agotar los reintentos, PasarelaGateway ya tradujo la falla técnica
        // a un DomainException: el handler solo conoce el contrato de Core.
        var gateway = new StubPasarelaGateway(
            new CobroResponse { Estado = "APROBADO" },
            new DomainException(Errores.PASARELA_NO_DISPONIBLE, "La pasarela no respondió."));
        var handler = new ProcesarCobroCommandHandler(new StubCitaService(true), pagoService, gateway);

        var result = await handler.Handle(Command(), CancellationToken.None);

        Assert.Equal("TX-INTERNA", result.Data);
    }
}