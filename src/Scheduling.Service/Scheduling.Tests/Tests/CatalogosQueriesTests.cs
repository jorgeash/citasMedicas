using Scheduling.Core.Features.Scheduling.EstadosCita.Queries.GetEstadosCita;
using Scheduling.Core.Features.Scheduling.Medicos.Queries.GetMedicos;
using Scheduling.Domain.Entities;
using Scheduling.Tests.Fakes;

namespace Scheduling.Tests.Tests;

public class CatalogosQueriesTests
{
    [Fact]
    public async Task GetMedicos_DevuelveElListadoPaginadoConElNombreCompleto()
    {
        var medicos = new FakeMedicoRepository();
        medicos.Seed(
            new Medico
            {
                MedicoID = 1,
                CodigoMedico = "MED-001",
                Nombres = "Ana María",
                Apellidos = "Gómez",
                Activo = true
            },
            new Medico
            {
                MedicoID = 2,
                CodigoMedico = "MED-002",
                Nombres = "Luis",
                Apellidos = "Torres",
                Activo = true
            });

        var handler = new GetMedicosQueryHandler(medicos);
        var result = await handler.Handle(new GetMedicosQuery(), CancellationToken.None);

        Assert.Equal(2, result.Data.TotalRecords);
        Assert.Equal(2, result.Data.Data.Count);

        var primero = result.Data.Data.First(m => m.MedicoID == 1);
        Assert.Equal("Ana María Gómez", primero.Nombre);
        Assert.Equal("MED-001", primero.CodigoMedico);
        Assert.True(primero.Activo);
    }

    [Fact]
    public async Task GetEstadosCita_DevuelveElCatalogoCompletoOrdenadoPorId()
    {
        var estados = new FakeEstadoCitaRepository();
        estados.Seed(
            new EstadoCita { EstadoCitaID = 3, Codigo = "CANCELADA", Nombre = "Cancelada" },
            new EstadoCita { EstadoCitaID = 1, Codigo = "PROGRAMADA", Nombre = "Programada" },
            new EstadoCita { EstadoCitaID = 2, Codigo = "ATENDIDA", Nombre = "Atendida" });

        var handler = new GetEstadosCitaQueryHandler(estados);
        var result = await handler.Handle(new GetEstadosCitaQuery(), CancellationToken.None);

        Assert.Equal(3, result.Data.Count);
        Assert.Equal(new[] { 1, 2, 3 }, result.Data.Select(e => e.EstadoCitaID));
        Assert.Equal("PROGRAMADA", result.Data[0].Codigo);
        Assert.Equal("Programada", result.Data[0].Nombre);
    }
}
