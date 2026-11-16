using Billing.Core.DTOs.External;
using Billing.Core.Exceptions;
using Billing.Core.Features.Billing.Facturas.Commands.CreateFactura;
using Billing.Core.Services;
using Billing.Domain.Entities;
using Billing.Tests.Fakes;

namespace Billing.Tests.Tests;

public class CreateFacturaCommandHandlerTests
{
    private sealed class SchedulingStub : ISchedulingClient
    {
        private readonly PacienteExternoDto? _paciente;
        private readonly CitaExternaDto? _cita;

        public SchedulingStub(PacienteExternoDto? paciente, CitaExternaDto? cita = null)
        {
            _paciente = paciente;
            _cita = cita;
        }

        public Task<PacienteExternoDto?> GetPacienteAsync(int pacienteId, CancellationToken cancellationToken = default)
            => Task.FromResult(_paciente);

        public Task<CitaExternaDto?> GetCitaAsync(int citaId, CancellationToken cancellationToken = default)
            => Task.FromResult(_cita);
    }

    private static (CreateFacturaCommandHandler handler, InMemoryRepository<Factura> facturas) BuildHandler(
        Tarifa tarifa,
        PacienteExternoDto? paciente = null,
        CitaExternaDto? cita = null)
    {
        var facturas = new FakeFacturaRepository();
        var tarifas = new FakeTarifaRepository();
        tarifas.Seed(tarifa);

        var handler = new CreateFacturaCommandHandler(
            facturas,
            tarifas,
            new SchedulingStub(paciente ?? new PacienteExternoDto { PacienteID = 15, Nombre = "Juan Pérez" }, cita));

        return (handler, facturas);
    }

    private static Tarifa TarifaValida() => new()
    {
        TarifaID = 1,
        ServicioID = 10,
        NombreTarifa = "Consulta particular",
        Precio = 50m,
        Moneda = "USD",
        FechaInicio = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
        Activa = true,
        Servicio = new Servicio { ServicioID = 10, Nombre = "Consulta general", Activo = true }
    };

    [Fact]
    public async Task Handle_CuandoTodoEsValido_CalculaTotalesYGuardaSnapshot()
    {
        var (handler, facturas) = BuildHandler(TarifaValida());

        var result = await handler.Handle(new CreateFacturaCommand
        {
            PacienteID = 15,
            Moneda = "USD",
            Detalle = new List<CreateFacturaItem> { new() { TarifaID = 1, Cantidad = 2 } }
        }, CancellationToken.None);

        Assert.Equal("Juan Pérez", result.PacienteNombre);
        Assert.Equal(100m, result.Subtotal);
        Assert.Equal(100m, result.Total);
        Assert.Equal(100m, result.Saldo);
        Assert.Equal("PENDIENTE", result.Estado);
        Assert.StartsWith("F-", result.Numero);

        var detalle = Assert.Single(result.Detalle);
        Assert.Equal("Consulta general", detalle.Descripcion);
        Assert.Equal(50m, detalle.PrecioUnitario);
        Assert.Equal(100m, detalle.Subtotal);

        var stored = await facturas.GetAllAsync();
        Assert.Single(stored);
    }

    [Fact]
    public async Task Handle_CuandoElPacienteNoExiste_LanzaDomainException()
    {
        var facturas = new FakeFacturaRepository();
        var tarifas = new FakeTarifaRepository();
        tarifas.Seed(TarifaValida());

        var handler = new CreateFacturaCommandHandler(
            facturas,
            tarifas,
            new SchedulingStub(null));

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(new CreateFacturaCommand
        {
            PacienteID = 99,
            Detalle = new List<CreateFacturaItem> { new() { TarifaID = 1, Cantidad = 1 } }
        }, CancellationToken.None));

        Assert.Equal(Errores.PACIENTE_NO_ENCONTRADO, ex.Codigo);
    }

    [Fact]
    public async Task Handle_CuandoLaTarifaEstaInactiva_LanzaDomainException()
    {
        var tarifa = TarifaValida();
        tarifa.Activa = false;
        var (handler, _) = BuildHandler(tarifa);

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(new CreateFacturaCommand
        {
            PacienteID = 15,
            Detalle = new List<CreateFacturaItem> { new() { TarifaID = 1, Cantidad = 1 } }
        }, CancellationToken.None));

        Assert.Equal(Errores.TARIFA_NO_VALIDA, ex.Codigo);
    }

    [Fact]
    public async Task Handle_CuandoNoHayDetalle_LanzaDomainException()
    {
        var (handler, _) = BuildHandler(TarifaValida());

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(new CreateFacturaCommand
        {
            PacienteID = 15
        }, CancellationToken.None));

        Assert.Equal(Errores.FACTURA_SIN_DETALLE, ex.Codigo);
    }

    [Fact]
    public async Task Handle_CuandoLaCitaEsValida_GuardaElIdDeLaCita()
    {
        var (handler, facturas) = BuildHandler(
            TarifaValida(),
            cita: new CitaExternaDto { CitaID = 7, PacienteID = 15, Fecha = DateTime.UtcNow });

        var result = await handler.Handle(new CreateFacturaCommand
        {
            PacienteID = 15,
            CitaID = 7,
            Detalle = new List<CreateFacturaItem> { new() { TarifaID = 1, Cantidad = 1 } }
        }, CancellationToken.None);

        Assert.Equal(7, result.CitaID);

        var stored = await facturas.GetAllAsync();
        Assert.Equal(7, Assert.Single(stored).CitaID);
    }

    [Fact]
    public async Task Handle_CuandoLaCitaNoExiste_LanzaDomainException()
    {
        // El stub sin cita devuelve null -> "no encontrada".
        var (handler, _) = BuildHandler(TarifaValida());

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(new CreateFacturaCommand
        {
            PacienteID = 15,
            CitaID = 999,
            Detalle = new List<CreateFacturaItem> { new() { TarifaID = 1, Cantidad = 1 } }
        }, CancellationToken.None));

        Assert.Equal(Errores.CITA_NO_ENCONTRADA, ex.Codigo);
    }

    [Fact]
    public async Task Handle_CuandoLaCitaEsDeOtroPaciente_LanzaDomainException()
    {
        var (handler, _) = BuildHandler(
            TarifaValida(),
            cita: new CitaExternaDto { CitaID = 1, PacienteID = 2, Fecha = DateTime.UtcNow });

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(new CreateFacturaCommand
        {
            PacienteID = 15,
            CitaID = 1,
            Detalle = new List<CreateFacturaItem> { new() { TarifaID = 1, Cantidad = 1 } }
        }, CancellationToken.None));

        Assert.Equal(Errores.CITA_PACIENTE_INCONSISTENTE, ex.Codigo);
    }
}
