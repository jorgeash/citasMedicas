using Scheduling.Core.Features.Scheduling.Citas.Queries.GetCitaById;
using Scheduling.Core.Features.Scheduling.Citas.Queries.GetCitas;
using Scheduling.Domain.Entities;
using Scheduling.Tests.Fakes;

namespace Scheduling.Tests.Tests;

public class CitaQueriesTests
{
    private static Cita CitaCompleta() => new()
    {
        CitaID = 7,
        PacienteID = 1,
        MedicoID = 2,
        EstadoCitaID = 1,
        FechaHoraInicio = new DateTime(2026, 11, 10, 9, 0, 0),
        Paciente = new Paciente { PacienteID = 1, Nombres = "Juan", Apellidos = "Pérez" },
        Medico = new Medico { MedicoID = 2, Nombres = "Ana", Apellidos = "Gómez" },
        EstadoCita = new EstadoCita { EstadoCitaID = 1, Codigo = "PROGRAMADA", Nombre = "Programada" }
    };

    [Fact]
    public async Task GetCitaById_MapeaFechaYNombreDelPaciente()
    {
        var citas = new FakeCitaRepository();
        citas.Seed(CitaCompleta());

        var handler = new GetCitaByIdQueryHandler(citas);
        var result = await handler.Handle(new GetCitaByIdQuery(7), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(7, result!.CitaID);
        Assert.Equal(1, result.PacienteID);
        Assert.Equal("Juan Pérez", result.PacienteNombre);
        Assert.Equal("Ana Gómez", result.MedicoNombre);
        Assert.Equal(new DateTime(2026, 11, 10, 9, 0, 0), result.Fecha);
        Assert.Equal("Programada", result.EstadoCita);
    }

    [Fact]
    public async Task GetCitaById_CuandoNoExiste_DevuelveNull()
    {
        var handler = new GetCitaByIdQueryHandler(new FakeCitaRepository());

        var result = await handler.Handle(new GetCitaByIdQuery(404), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetCitas_FiltraPorPaciente()
    {
        var citas = new FakeCitaRepository();
        citas.Seed(
            CitaCompleta(),
            new Cita
            {
                CitaID = 8,
                PacienteID = 2,
                MedicoID = 2,
                EstadoCitaID = 1,
                FechaHoraInicio = new DateTime(2026, 11, 11, 10, 0, 0)
            });

        var handler = new GetCitasQueryHandler(citas);
        var result = await handler.Handle(new GetCitasQuery { PacienteID = 1 }, CancellationToken.None);

        Assert.Equal(1, result.Data.TotalRecords);
        Assert.Equal(7, Assert.Single(result.Data.Data).CitaID);
    }
}
