using Scheduling.Core.Exceptions;
using Scheduling.Core.Features.Scheduling.Citas.Commands.CancelarCita;
using Scheduling.Domain.Entities;
using Scheduling.Tests.Fakes;

namespace Scheduling.Tests.Tests;

public class CancelarCitaCommandHandlerTests
{
    private static FakeEstadoCitaRepository Estados()
    {
        var repo = new FakeEstadoCitaRepository();
        repo.Seed(
            new EstadoCita { EstadoCitaID = 1, Codigo = "PROGRAMADA", Nombre = "Programada" },
            new EstadoCita { EstadoCitaID = 3, Codigo = "CANCELADA", Nombre = "Cancelada" });
        return repo;
    }

    [Fact]
    public async Task Handle_CuandoLaCitaEstaProgramada_LaCancela()
    {
        var citas = new FakeCitaRepository();
        citas.Seed(new Cita { CitaID = 10, PacienteID = 1, MedicoID = 2, EstadoCitaID = 1 });

        var handler = new CancelarCitaCommandHandler(citas, Estados());

        var result = await handler.Handle(new CancelarCitaCommand { CitaID = 10 }, CancellationToken.None);

        Assert.Equal(3, result.EstadoCitaID);
    }

    [Fact]
    public async Task Handle_CuandoLaCitaYaEstaCancelada_LanzaDomainException()
    {
        var citas = new FakeCitaRepository();
        citas.Seed(new Cita { CitaID = 10, PacienteID = 1, MedicoID = 2, EstadoCitaID = 3 });

        var handler = new CancelarCitaCommandHandler(citas, Estados());

        var ex = await Assert.ThrowsAsync<DomainException>(
            () => handler.Handle(new CancelarCitaCommand { CitaID = 10 }, CancellationToken.None));

        Assert.Equal(Errores.CITA_YA_CANCELADA, ex.Codigo);
    }

    [Fact]
    public async Task Handle_CuandoLaCitaNoExiste_LanzaDomainException()
    {
        var handler = new CancelarCitaCommandHandler(new FakeCitaRepository(), Estados());

        var ex = await Assert.ThrowsAsync<DomainException>(
            () => handler.Handle(new CancelarCitaCommand { CitaID = 404 }, CancellationToken.None));

        Assert.Equal(Errores.CITA_NO_ENCONTRADA, ex.Codigo);
    }
}
