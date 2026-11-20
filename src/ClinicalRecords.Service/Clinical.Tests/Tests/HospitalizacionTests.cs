using Clinical.Core.Exceptions;
using Clinical.Core.Features.Clinical.Hospitalizacion.Commands.EgresarPaciente;
using Clinical.Core.Features.Clinical.Hospitalizacion.Commands.IngresarPaciente;
using Clinical.Core.Features.Clinical.Hospitalizacion.Queries.GetHospitalizaciones;
using Clinical.Domain.Entities;
using Clinical.Tests.Fakes;

namespace Clinical.Tests.Tests;

public class HospitalizacionTests
{
    [Fact]
    public async Task IngresarPaciente_CuandoTodoEsValido_CreaIngresoYOcupaCama()
    {
        var (handler, hospitalizaciones, camas) = BuildIngresoHandler();

        var result = await handler.Handle(new IngresarPacienteCommand
        {
            ExpedienteClinicoID = 1,
            AtencionID = 10,
            CamaID = 1,
            Motivo = "Observación postoperatoria"
        }, CancellationToken.None);

        Assert.Equal("ACTIVA", result.Estado);
        Assert.Single(await hospitalizaciones.GetAllAsync());
        Assert.Equal("OCUPADA", (await camas.GetByIdAsync(1))!.Estado);
    }

    [Fact]
    public async Task IngresarPaciente_CuandoLaCamaEstaOcupada_LanzaCamaNoDisponible()
    {
        var (handler, _, camas) = BuildIngresoHandler();
        (await camas.GetByIdAsync(1))!.Estado = "OCUPADA";

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(new IngresarPacienteCommand
        {
            ExpedienteClinicoID = 1,
            AtencionID = 10,
            CamaID = 1,
            Motivo = "Observación"
        }, CancellationToken.None));

        Assert.Equal(Errores.CAMA_NO_DISPONIBLE, ex.Codigo);
    }

    [Fact]
    public async Task IngresarPaciente_CuandoYaHayIngresoActivo_LanzaHospitalizacionActivaPrevia()
    {
        var (handler, hospitalizaciones, _) = BuildIngresoHandler();
        hospitalizaciones.Seed(new Hospitalizacion
        {
            HospitalizacionID = 5,
            ExpedienteClinicoID = 1,
            AtencionID = 10,
            CamaID = 2,
            FechaIngreso = DateTime.UtcNow.AddDays(-2),
            Motivo = "Ingreso anterior",
            Estado = "ACTIVA"
        });

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(new IngresarPacienteCommand
        {
            ExpedienteClinicoID = 1,
            AtencionID = 10,
            CamaID = 1,
            Motivo = "Nuevo ingreso"
        }, CancellationToken.None));

        Assert.Equal(Errores.HOSPITALIZACION_ACTIVA_PREVIA, ex.Codigo);
    }

    [Fact]
    public async Task EgresarPaciente_CuandoEsValido_LiberaLaCama()
    {
        var hospitalizaciones = new FakeHospitalizacionRepository();
        hospitalizaciones.Seed(new Hospitalizacion
        {
            HospitalizacionID = 5,
            ExpedienteClinicoID = 1,
            AtencionID = 10,
            CamaID = 1,
            FechaIngreso = DateTime.UtcNow.AddDays(-1),
            Motivo = "Observación",
            Estado = "ACTIVA"
        });
        var camas = new FakeCamaRepository();
        camas.Seed(new Cama { CamaID = 1, HabitacionID = 1, Codigo = "C-101", Estado = "OCUPADA", Activa = true });
        var handler = new EgresarPacienteCommandHandler(hospitalizaciones, camas);

        var result = await handler.Handle(new EgresarPacienteCommand
        {
            HospitalizacionID = 5,
            FechaEgreso = DateTime.UtcNow
        }, CancellationToken.None);

        Assert.Equal("EGRESADA", result.Estado);
        Assert.NotNull(result.FechaEgreso);
        Assert.Equal("LIBRE", (await camas.GetByIdAsync(1))!.Estado);
    }

    [Fact]
    public async Task EgresarPaciente_CuandoLaFechaEsAnteriorAlIngreso_LanzaFechaEgresoInvalida()
    {
        var ingreso = DateTime.UtcNow.AddDays(-1);
        var hospitalizaciones = new FakeHospitalizacionRepository();
        hospitalizaciones.Seed(new Hospitalizacion
        {
            HospitalizacionID = 5,
            ExpedienteClinicoID = 1,
            AtencionID = 10,
            CamaID = 1,
            FechaIngreso = ingreso,
            Motivo = "Observación",
            Estado = "ACTIVA"
        });
        var handler = new EgresarPacienteCommandHandler(hospitalizaciones, new FakeCamaRepository());

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(
            new EgresarPacienteCommand { HospitalizacionID = 5, FechaEgreso = ingreso.AddDays(-2) },
            CancellationToken.None));

        Assert.Equal(Errores.FECHA_EGRESO_INVALIDA, ex.Codigo);
    }

    [Fact]
    public async Task GetHospitalizaciones_SoloActivas_FiltraLasEgresadas()
    {
        var repo = new FakeHospitalizacionRepository();
        repo.Seed(
            new Hospitalizacion { HospitalizacionID = 5, ExpedienteClinicoID = 1, AtencionID = 10, FechaIngreso = DateTime.UtcNow, Motivo = "Activa", Estado = "ACTIVA" },
            new Hospitalizacion { HospitalizacionID = 6, ExpedienteClinicoID = 2, AtencionID = 11, FechaIngreso = DateTime.UtcNow, Motivo = "Egresada", Estado = "EGRESADA" });
        var handler = new GetHospitalizacionesQueryHandler(repo);

        var result = await handler.Handle(new GetHospitalizacionesQuery { SoloActivas = true }, CancellationToken.None);

        Assert.Equal(1, result.TotalRecords);
        Assert.Equal(5, Assert.Single(result.Data).HospitalizacionID);
    }

    private static (
        IngresarPacienteCommandHandler handler,
        FakeHospitalizacionRepository hospitalizaciones,
        FakeCamaRepository camas) BuildIngresoHandler()
    {
        var hospitalizaciones = new FakeHospitalizacionRepository();

        var expedientes = new FakeExpedienteClinicoRepository();
        expedientes.Seed(new ExpedienteClinico
        {
            ExpedienteClinicoID = 1,
            PacienteID = 1,
            NumeroExpediente = "EXP-2026-000001",
            Activo = true
        });

        var atenciones = new FakeAtencionRepository();
        atenciones.Seed(new Atencion { AtencionID = 10, PacienteID = 1, ExpedienteClinicoID = 1, Estado = "EN_CURSO" });

        var camas = new FakeCamaRepository();
        camas.Seed(new Cama { CamaID = 1, HabitacionID = 1, Codigo = "C-101", Estado = "LIBRE", Activa = true });

        return (new IngresarPacienteCommandHandler(hospitalizaciones, expedientes, atenciones, camas), hospitalizaciones, camas);
    }
}
