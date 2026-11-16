using Scheduling.Core.Exceptions;
using Scheduling.Core.Features.Scheduling.Citas.Commands.CreateCita;
using Scheduling.Domain.Entities;
using Scheduling.Tests.Fakes;

namespace Scheduling.Tests.Tests;

public class CreateCitaCommandHandlerTests
{
    private static FakePacienteRepository Pacientes()
    {
        var repo = new FakePacienteRepository();
        repo.Seed(new Paciente { PacienteID = 1, Nombres = "Juan", Apellidos = "Pérez" });
        return repo;
    }

    private static FakeMedicoRepository Medicos()
    {
        var repo = new FakeMedicoRepository();
        repo.Seed(new Medico { MedicoID = 2, Nombres = "Ana", Apellidos = "Gómez" });
        return repo;
    }

    private static FakeEstadoCitaRepository Estados()
    {
        var repo = new FakeEstadoCitaRepository();
        repo.Seed(
            new EstadoCita { EstadoCitaID = 1, Codigo = "PROGRAMADA", Nombre = "Programada" },
            new EstadoCita { EstadoCitaID = 3, Codigo = "CANCELADA", Nombre = "Cancelada" });
        return repo;
    }

    private static CreateCitaCommand ComandoValido() => new()
    {
        PacienteID = 1,
        MedicoID = 2,
        FechaHoraInicio = DateTime.UtcNow.AddDays(1)
    };

    [Fact]
    public async Task Handle_CuandoNoSeIndicaEstado_UsaProgramada()
    {
        var citas = new FakeCitaRepository();
        var handler = new CreateCitaCommandHandler(citas, Pacientes(), Medicos(), Estados());

        var id = await handler.Handle(ComandoValido(), CancellationToken.None);

        var cita = await citas.GetOneByAsync(c => c.CitaID == id);
        Assert.NotNull(cita);
        Assert.Equal(1, cita!.EstadoCitaID);
        Assert.Equal(1, cita.PacienteID);
    }

    [Fact]
    public async Task Handle_CuandoElPacienteNoExiste_LanzaDomainException()
    {
        var handler = new CreateCitaCommandHandler(
            new FakeCitaRepository(),
            new FakePacienteRepository(),
            Medicos(),
            Estados());

        var command = ComandoValido();
        command.PacienteID = 99;

        var ex = await Assert.ThrowsAsync<DomainException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(Errores.PACIENTE_NO_ENCONTRADO, ex.Codigo);
    }

    [Fact]
    public async Task Handle_CuandoElMedicoNoExiste_LanzaDomainException()
    {
        var handler = new CreateCitaCommandHandler(
            new FakeCitaRepository(),
            Pacientes(),
            new FakeMedicoRepository(),
            Estados());

        var ex = await Assert.ThrowsAsync<DomainException>(
            () => handler.Handle(ComandoValido(), CancellationToken.None));

        Assert.Equal(Errores.MEDICO_NO_ENCONTRADO, ex.Codigo);
    }

    [Fact]
    public async Task Handle_CuandoLaFechaFinEsAnteriorAlInicio_LanzaDomainException()
    {
        var handler = new CreateCitaCommandHandler(
            new FakeCitaRepository(),
            Pacientes(),
            Medicos(),
            Estados());

        var command = ComandoValido();
        command.FechaHoraFin = command.FechaHoraInicio.AddHours(-1);

        var ex = await Assert.ThrowsAsync<DomainException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(Errores.DATOS_INVALIDOS, ex.Codigo);
    }
}
