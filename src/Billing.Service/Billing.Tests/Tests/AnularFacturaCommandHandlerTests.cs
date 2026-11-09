using Billing.Core.Exceptions;
using Billing.Core.Features.Billing.Facturas.Commands.AnularFactura;
using Billing.Domain.Entities;
using Billing.Domain.Enums;
using Billing.Tests.Fakes;

namespace Billing.Tests.Tests;

public class AnularFacturaCommandHandlerTests
{
    private static FakeFacturaRepository RepositoryWith(Factura factura)
    {
        var repository = new FakeFacturaRepository();
        repository.Seed(factura);
        return repository;
    }

    [Fact]
    public async Task Handle_CuandoLaFacturaEstaPendiente_LaAnulaYDejaSaldoCero()
    {
        var factura = new Factura
        {
            FacturaID = 1,
            Numero = "F-2026-000001",
            Total = 80m,
            Saldo = 80m,
            Estado = EstadoFactura.PENDIENTE
        };
        var handler = new AnularFacturaCommandHandler(RepositoryWith(factura));

        var result = await handler.Handle(new AnularFacturaCommand
        {
            FacturaID = 1,
            Motivo = "Error de cobro"
        }, CancellationToken.None);

        Assert.Equal("ANULADA", result.Estado);
        Assert.Equal(0m, result.Saldo);
        Assert.Contains("ANULADA: Error de cobro", result.Observaciones);
    }

    [Fact]
    public async Task Handle_CuandoLaFacturaYaFuePagada_LanzaDomainException()
    {
        var factura = new Factura
        {
            FacturaID = 1,
            Numero = "F-2026-000001",
            Total = 80m,
            Saldo = 0m,
            Estado = EstadoFactura.PAGADA
        };
        var handler = new AnularFacturaCommandHandler(RepositoryWith(factura));

        var ex = await Assert.ThrowsAsync<DomainException>(
            () => handler.Handle(new AnularFacturaCommand { FacturaID = 1 }, CancellationToken.None));

        Assert.Equal(Errores.FACTURA_YA_PAGADA, ex.Codigo);
    }

    [Fact]
    public async Task Handle_CuandoLaFacturaNoExiste_LanzaDomainException()
    {
        var handler = new AnularFacturaCommandHandler(
            new FakeFacturaRepository());

        var ex = await Assert.ThrowsAsync<DomainException>(
            () => handler.Handle(new AnularFacturaCommand { FacturaID = 77 }, CancellationToken.None));

        Assert.Equal(Errores.FACTURA_NO_ENCONTRADA, ex.Codigo);
    }
}
