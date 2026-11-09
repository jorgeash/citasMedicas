using Billing.Core.DTOs;
using Billing.Core.Exceptions;
using Billing.Core.Features.Billing.Pagos.Commands.ProcesarPago;
using Billing.Core.Services;
using NugetPackage_Rest.Exceptions;

namespace Billing.Tests.Tests;

public class ProcesarPagoCommandHandlerTests
{
    private sealed class PagoServiceStub : IPagoService
    {
        private readonly Func<Task<string>> _behavior;
        public PagoServiceStub(Func<Task<string>> behavior) => _behavior = behavior;

        public Task<string> ProcesarPagoAsync(Guid facturaId, decimal monto, string moneda, CancellationToken cancellationToken = default)
            => _behavior();
    }

    [Fact]
    public async Task Handle_CuandoElServicioDeFalla_LanzaDomainExceptionConCodigoPagoFallido()
    {
        var stub = new PagoServiceStub(() => throw new ApiException(
            "Request POST https://pasarela/api/Pagos/procesar failed due to a network error: refused",
            ApiFailureReason.Network));
        var handler = new ProcesarPagoCommandHandler(stub);

        var ex = await Assert.ThrowsAsync<DomainException>(
            () => handler.Handle(new ProcesarPagoCommand { FacturaId = Guid.NewGuid(), Monto = 9.99m, Moneda = "USD" }, CancellationToken.None));

        Assert.Equal(Errores.PAGO_FALLIDO, ex.Codigo);
        Assert.Equal(601, (int)ex.Codigo);
        Assert.Contains("No fue posible procesar el pago", ex.Message);
    }

    [Fact]
    public async Task Handle_CuandoElPagoSeProcesa_DevuelveLaTransaccionEnElSobre()
    {
        var stub = new PagoServiceStub(() => Task.FromResult("TX-0002"));
        var handler = new ProcesarPagoCommandHandler(stub);

        var result = await handler.Handle(
            new ProcesarPagoCommand { FacturaId = Guid.NewGuid(), Monto = 100m, Moneda = "USD" },
            CancellationToken.None);

        Assert.IsType<HttpResponse<string>>(result);
        Assert.Equal("TX-0002", result.Data);
    }

    [Fact]
    public async Task Handle_CuandoElServicioLanzaDomainException_NoLaVuelveATraducir()
    {
        var original = new DomainException(Errores.PASARELA_NO_DISPONIBLE, "sin pasarela");
        var stub = new PagoServiceStub(() => throw original);
        var handler = new ProcesarPagoCommandHandler(stub);

        var ex = await Assert.ThrowsAsync<DomainException>(
            () => handler.Handle(new ProcesarPagoCommand { FacturaId = Guid.NewGuid(), Monto = 1m }, CancellationToken.None));

        Assert.Same(original, ex);
        Assert.Equal(Errores.PASARELA_NO_DISPONIBLE, ex.Codigo);
    }
}
