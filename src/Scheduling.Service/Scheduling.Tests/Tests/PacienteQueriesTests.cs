using Scheduling.Core.Features.Scheduling.Pacientes.Queries.GetPacienteById;
using Scheduling.Domain.Entities;
using Scheduling.Tests.Fakes;

namespace Scheduling.Tests.Tests;

public class PacienteQueriesTests
{
    [Fact]
    public async Task GetPacienteById_DevuelveLaAseguradoraActivaVigente()
    {
        var hoy = DateOnly.FromDateTime(DateTime.UtcNow);

        var paciente = new Paciente
        {
            PacienteID = 1,
            Nombres = "Juan",
            Apellidos = "Pérez",
            Seguros =
            {
                new PacienteAseguradora
                {
                    PacienteAseguradoraID = 1,
                    AseguradoraID = 10,
                    Activo = false,
                    FechaInicio = hoy.AddDays(-30)
                },
                new PacienteAseguradora
                {
                    PacienteAseguradoraID = 2,
                    AseguradoraID = 20,
                    Activo = true,
                    FechaInicio = hoy.AddDays(-30),
                    FechaFin = hoy.AddDays(-1)
                },
                new PacienteAseguradora
                {
                    PacienteAseguradoraID = 3,
                    AseguradoraID = 30,
                    Activo = true,
                    FechaInicio = hoy.AddDays(-1)
                }
            }
        };

        var pacientes = new FakePacienteRepository();
        pacientes.Seed(paciente);

        var handler = new GetPacienteByIdQueryHandler(pacientes);
        var result = await handler.Handle(new GetPacienteByIdQuery(1), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Juan Pérez", result!.Nombre);
        Assert.Equal(30, result.AseguradoraID);
    }

    [Fact]
    public async Task GetPacienteById_CuandoNoTieneSeguro_DevuelveNullEnAseguradora()
    {
        var pacientes = new FakePacienteRepository();
        pacientes.Seed(new Paciente { PacienteID = 2, Nombres = "María", Apellidos = "López" });

        var handler = new GetPacienteByIdQueryHandler(pacientes);
        var result = await handler.Handle(new GetPacienteByIdQuery(2), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Null(result!.AseguradoraID);
    }

    [Fact]
    public async Task GetPacienteById_CuandoNoExiste_DevuelveNull()
    {
        var handler = new GetPacienteByIdQueryHandler(new FakePacienteRepository());

        var result = await handler.Handle(new GetPacienteByIdQuery(404), CancellationToken.None);

        Assert.Null(result);
    }
}
