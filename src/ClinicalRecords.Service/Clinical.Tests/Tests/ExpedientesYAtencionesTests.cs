using Clinical.Core.DTOs.External;
using Clinical.Core.Exceptions;
using Clinical.Core.Features.Clinical.Atenciones.Commands.CreateAtencion;
using Clinical.Core.Features.Clinical.Expedientes.Commands.CreateExpediente;
using Clinical.Core.Features.Clinical.Expedientes.Queries.GetExpedientes;
using Clinical.Core.Features.Clinical.SignosVitales.Commands.RegistrarSignosVitales;
using Clinical.Domain.Entities;
using Clinical.Tests.Fakes;

namespace Clinical.Tests.Tests;

public class ExpedientesYAtencionesTests
{
    [Fact]
    public async Task CreateExpediente_CuandoEsValido_GeneraNumeroConAnioYId()
    {
        var repo = new FakeExpedienteClinicoRepository();
        var handler = new CreateExpedienteCommandHandler(repo);

        var result = await handler.Handle(new CreateExpedienteCommand
        {
            PacienteID = 1,
            FechaApertura = new DateOnly(2026, 1, 15)
        }, CancellationToken.None);

        Assert.Equal("EXP-2026-000001", result.NumeroExpediente);
        Assert.True(result.Activo);
        Assert.Single(await repo.GetAllAsync());
    }

    [Fact]
    public async Task CreateExpediente_CuandoYaExisteActivo_LanzaExpedienteDuplicado()
    {
        var repo = new FakeExpedienteClinicoRepository();
        repo.Seed(new ExpedienteClinico
        {
            ExpedienteClinicoID = 1,
            PacienteID = 2,
            NumeroExpediente = "EXP-2025-000001",
            Activo = true
        });
        var handler = new CreateExpedienteCommandHandler(repo);

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(
            new CreateExpedienteCommand { PacienteID = 2 }, CancellationToken.None));

        Assert.Equal(Errores.EXPEDIENTE_DUPLICADO, ex.Codigo);
    }

    [Fact]
    public async Task CreateAtencion_CuandoTodoEsValido_CreaExpedienteYAtencion()
    {
        var atenciones = new FakeAtencionRepository();
        var expedientes = new FakeExpedienteClinicoRepository();
        var handler = new CreateAtencionCommandHandler(
            atenciones,
            expedientes,
            new SchedulingStub(new CitaExternaDto { CitaID = 50, PacienteID = 1, Fecha = DateTime.UtcNow }),
            new IdentityStub(new MedicoExternoDto { MedicoID = 3, NombreCompleto = "Dra. Ana", Activo = true }));

        var result = await handler.Handle(new CreateAtencionCommand
        {
            CitaID = 50,
            PacienteID = 1,
            MedicoID = 3,
            Motivo = "Control general"
        }, CancellationToken.None);

        Assert.Equal("EN_CURSO", result.Estado);
        Assert.Equal(50, result.CitaID);
        Assert.NotNull(result.ExpedienteClinicoID);
        Assert.Single(await expedientes.GetAllAsync());
        Assert.Single(await atenciones.GetAllAsync());
    }

    [Fact]
    public async Task CreateAtencion_CuandoLaCitaNoExiste_LanzaCitaNoEncontrada()
    {
        var handler = new CreateAtencionCommandHandler(
            new FakeAtencionRepository(),
            new FakeExpedienteClinicoRepository(),
            new SchedulingStub(null),
            new IdentityStub());

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(
            new CreateAtencionCommand { CitaID = 99, PacienteID = 1, MedicoID = 3 }, CancellationToken.None));

        Assert.Equal(Errores.CITA_NO_ENCONTRADA, ex.Codigo);
    }

    [Fact]
    public async Task CreateAtencion_CuandoLaCitaEsDeOtroPaciente_LanzaInconsistencia()
    {
        var handler = new CreateAtencionCommandHandler(
            new FakeAtencionRepository(),
            new FakeExpedienteClinicoRepository(),
            new SchedulingStub(new CitaExternaDto { CitaID = 50, PacienteID = 2, Fecha = DateTime.UtcNow }),
            new IdentityStub());

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(
            new CreateAtencionCommand { CitaID = 50, PacienteID = 1, MedicoID = 3 }, CancellationToken.None));

        Assert.Equal(Errores.CITA_PACIENTE_INCONSISTENTE, ex.Codigo);
    }

    [Fact]
    public async Task CreateAtencion_CuandoElMedicoEstaInactivo_LanzaMedicoNoValido()
    {
        var handler = new CreateAtencionCommandHandler(
            new FakeAtencionRepository(),
            new FakeExpedienteClinicoRepository(),
            new SchedulingStub(new CitaExternaDto { CitaID = 50, PacienteID = 1, Fecha = DateTime.UtcNow }),
            new IdentityStub(new MedicoExternoDto { MedicoID = 3, Activo = false }));

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(
            new CreateAtencionCommand { CitaID = 50, PacienteID = 1, MedicoID = 3 }, CancellationToken.None));

        Assert.Equal(Errores.MEDICO_NO_VALIDO, ex.Codigo);
    }

    [Fact]
    public async Task CreateAtencion_CuandoYaHayAtencionParaLaCita_LanzaDuplicada()
    {
        var atenciones = new FakeAtencionRepository();
        atenciones.Seed(new Atencion { AtencionID = 7, CitaID = 50, PacienteID = 1, Estado = "EN_CURSO" });
        var handler = new CreateAtencionCommandHandler(
            atenciones,
            new FakeExpedienteClinicoRepository(),
            new SchedulingStub(new CitaExternaDto { CitaID = 50, PacienteID = 1, Fecha = DateTime.UtcNow }),
            new IdentityStub());

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(
            new CreateAtencionCommand { CitaID = 50, PacienteID = 1, MedicoID = 3 }, CancellationToken.None));

        Assert.Equal(Errores.ATENCION_CITA_DUPLICADA, ex.Codigo);
    }

    [Fact]
    public async Task RegistrarSignosVitales_CuandoHayPesoYTalla_CalculaIMC()
    {
        var atenciones = new FakeAtencionRepository();
        atenciones.Seed(new Atencion { AtencionID = 10, PacienteID = 1, Estado = "EN_CURSO" });
        var handler = new RegistrarSignosVitalesCommandHandler(new FakeSignosVitalesRepository(), atenciones);

        var result = await handler.Handle(new RegistrarSignosVitalesCommand
        {
            AtencionID = 10,
            PesoKg = 70m,
            TallaCm = 175m,
            Temperatura = 36.5m
        }, CancellationToken.None);

        Assert.Equal(22.86m, result.IMC);
    }

    [Fact]
    public async Task RegistrarSignosVitales_CuandoNoExisteAtencion_LanzaAtencionNoEncontrada()
    {
        var handler = new RegistrarSignosVitalesCommandHandler(
            new FakeSignosVitalesRepository(), new FakeAtencionRepository());

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(
            new RegistrarSignosVitalesCommand { AtencionID = 999 }, CancellationToken.None));

        Assert.Equal(Errores.ATENCION_NO_ENCONTRADA, ex.Codigo);
    }

    [Fact]
    public async Task GetExpedientes_BuscarSeCombinaConActivoEnAND()
    {
        var repo = new FakeExpedienteClinicoRepository();
        repo.Seed(
            new ExpedienteClinico { ExpedienteClinicoID = 1, PacienteID = 1, NumeroExpediente = "EXP-2026-000001", Observaciones = "Control diario", Activo = true },
            new ExpedienteClinico { ExpedienteClinicoID = 2, PacienteID = 2, NumeroExpediente = "EXP-2026-000002", Observaciones = "Control diario", Activo = false },
            new ExpedienteClinico { ExpedienteClinicoID = 3, PacienteID = 3, NumeroExpediente = "EXP-2026-000003", Observaciones = "Urgencia", Activo = true });
        var handler = new GetExpedientesQueryHandler(repo);

        var result = await handler.Handle(new GetExpedientesQuery
        {
            Buscar = "Control",
            Activo = true
        }, CancellationToken.None);

        Assert.Equal(1, result.TotalRecords);
        Assert.Equal(1, Assert.Single(result.Data).ExpedienteClinicoID);
    }
}
