using Scheduling.Core.Exceptions;
using Scheduling.Core.Features.Scheduling.Pacientes.Commands.CreatePaciente;
using Scheduling.Domain.Entities;
using Scheduling.Tests.Fakes;

namespace Scheduling.Tests.Tests;

public class CreatePacienteCommandHandlerTests
{
    private static CreatePacienteCommand ComandoValido() => new()
    {
        CodigoPaciente = "PAC-001",
        TipoDocumento = "DNI",
        NumeroDocumento = "12345678",
        Nombres = "Juan",
        Apellidos = "Pérez",
        FechaNacimiento = new DateOnly(1990, 5, 20),
        Sexo = "M"
    };

    [Fact]
    public async Task Handle_CuandoTodoEsValido_CreaElPacienteYSuSeguro()
    {
        var pacientes = new FakePacienteRepository();
        var aseguradoras = new FakeAseguradoraRepository();
        aseguradoras.Seed(new Aseguradora { AseguradoraID = 5, Codigo = "SEG", Nombre = "Seguros Salud" });

        var handler = new CreatePacienteCommandHandler(pacientes, aseguradoras);
        var command = ComandoValido();
        command.AseguradoraID = 5;
        command.NumeroPoliza = "POL-001";

        var id = await handler.Handle(command, CancellationToken.None);

        Assert.True(id > 0);

        var creado = await pacientes.GetOneByAsync(p => p.PacienteID == id);
        Assert.NotNull(creado);
        Assert.Equal("PAC-001", creado!.CodigoPaciente);
        var seguro = Assert.Single(creado.Seguros);
        Assert.Equal(5, seguro.AseguradoraID);
        Assert.True(seguro.Activo);
    }

    [Fact]
    public async Task Handle_CuandoElDocumentoYaExiste_LanzaDomainException()
    {
        var pacientes = new FakePacienteRepository();
        pacientes.Seed(new Paciente
        {
            PacienteID = 1,
            CodigoPaciente = "PAC-999",
            TipoDocumento = "DNI",
            NumeroDocumento = "12345678"
        });

        var handler = new CreatePacienteCommandHandler(pacientes, new FakeAseguradoraRepository());

        var ex = await Assert.ThrowsAsync<DomainException>(
            () => handler.Handle(ComandoValido(), CancellationToken.None));

        Assert.Equal(Errores.PACIENTE_DUPLICADO, ex.Codigo);
    }

    [Fact]
    public async Task Handle_CuandoLaAseguradoraNoExiste_LanzaDomainException()
    {
        var handler = new CreatePacienteCommandHandler(
            new FakePacienteRepository(),
            new FakeAseguradoraRepository());

        var command = ComandoValido();
        command.AseguradoraID = 99;
        command.NumeroPoliza = "POL-001";

        var ex = await Assert.ThrowsAsync<DomainException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(Errores.ASEGURADORA_NO_ENCONTRADA, ex.Codigo);
    }

    [Fact]
    public async Task Handle_CuandoFaltanDatosObligatorios_LanzaDomainException()
    {
        var handler = new CreatePacienteCommandHandler(
            new FakePacienteRepository(),
            new FakeAseguradoraRepository());

        var command = ComandoValido();
        command.Nombres = " ";

        var ex = await Assert.ThrowsAsync<DomainException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(Errores.DATOS_INVALIDOS, ex.Codigo);
    }
}
