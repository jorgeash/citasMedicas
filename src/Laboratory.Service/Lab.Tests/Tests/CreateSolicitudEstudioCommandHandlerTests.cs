using Lab.Core.DTOs.External;
using Lab.Core.Exceptions;
using Lab.Core.Features.Lab.SolicitudesEstudio.Commands.CreateSolicitudEstudio;
using Lab.Core.Services;
using Lab.Domain.Entities;
using Lab.Tests.Fakes;

namespace Lab.Tests.Tests;

public class CreateSolicitudEstudioCommandHandlerTests
{
    private static Estudio EstudioValido(bool activo = true) => new()
    {
        EstudioID = 1,
        Codigo = "HEMO",
        Nombre = "Hemograma completo",
        Tipo = "LABORATORIO",
        Activo = activo
    };

    private static CreateSolicitudEstudioCommand NuevaCommand() => new()
    {
        AtencionID = 10,
        PacienteID = 10,
        MedicoID = 5,
        Detalle = new List<CreateSolicitudEstudioItem>
        {
            new() { EstudioID = 1 }
        }
    };

    private static (
        CreateSolicitudEstudioCommandHandler handler,
        FakeSolicitudEstudioRepository solicitudes) BuildHandler(
            IClinicalRecordsClient? atenciones = null,
            IIdentityClient? medicos = null,
            params Estudio[] estudios)
    {
        var solicitudes = new FakeSolicitudEstudioRepository();
        var repositorioEstudios = new FakeEstudioRepository();
        repositorioEstudios.Seed(estudios.Length == 0 ? new[] { EstudioValido() } : estudios);

        var handler = new CreateSolicitudEstudioCommandHandler(
            solicitudes,
            repositorioEstudios,
            atenciones ?? new FakeClinicalRecordsClient(),
            medicos ?? new FakeIdentityClient());

        return (handler, solicitudes);
    }

    [Fact]
    public async Task Handle_SinDetalle_LanzaDomainException()
    {
        var (handler, _) = BuildHandler();
        var command = NuevaCommand();
        command.Detalle.Clear();

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(command, CancellationToken.None));

        Assert.Equal(Errores.SOLICITUD_SIN_DETALLE, ex.Codigo);
    }

    [Fact]
    public async Task Handle_AtencionInexistente_LanzaDomainException()
    {
        var (handler, _) = BuildHandler(new FakeClinicalRecordsClient(_ => null));

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            handler.Handle(NuevaCommand(), CancellationToken.None));

        Assert.Equal(Errores.ATENCION_NO_ENCONTRADA, ex.Codigo);
    }

    [Fact]
    public async Task Handle_AtencionDeOtroPaciente_LanzaDomainException()
    {
        var atenciones = new FakeClinicalRecordsClient(_ => new AtencionExternaDto
        {
            AtencionID = 10,
            PacienteID = 99,
            FechaAtencion = DateTime.UtcNow,
            Estado = "ATENDIDA"
        });
        var (handler, _) = BuildHandler(atenciones);

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            handler.Handle(NuevaCommand(), CancellationToken.None));

        Assert.Equal(Errores.ATENCION_PACIENTE_INCONSISTENTE, ex.Codigo);
    }

    [Fact]
    public async Task Handle_MedicoInactivo_LanzaDomainException()
    {
        var medicos = new FakeIdentityClient(_ => new MedicoExternoDto
        {
            MedicoID = 5,
            NombreCompleto = "Dr. Perez",
            Activo = false
        });
        var (handler, _) = BuildHandler(medicos: medicos);

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            handler.Handle(NuevaCommand(), CancellationToken.None));

        Assert.Equal(Errores.MEDICO_INACTIVO, ex.Codigo);
    }

    [Fact]
    public async Task Handle_DetalleDuplicado_LanzaDomainException()
    {
        var (handler, _) = BuildHandler();
        var command = NuevaCommand();
        command.Detalle.Add(new CreateSolicitudEstudioItem { EstudioID = 1 });

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(command, CancellationToken.None));

        Assert.Equal(Errores.DETALLE_DUPLICADO, ex.Codigo);
    }

    [Fact]
    public async Task Handle_EstudioInactivo_LanzaDomainException()
    {
        var (handler, _) = BuildHandler(estudios: new[] { EstudioValido(activo: false) });

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            handler.Handle(NuevaCommand(), CancellationToken.None));

        Assert.Equal(Errores.ESTUDIO_NO_ACTIVO, ex.Codigo);
    }

    [Fact]
    public async Task Handle_TodoValido_GeneraNumeroYEstadoPendiente()
    {
        var (handler, solicitudes) = BuildHandler();

        var result = await handler.Handle(NuevaCommand(), CancellationToken.None);

        Assert.StartsWith("SE-", result.NumeroSolicitud);
        Assert.EndsWith("-000001", result.NumeroSolicitud);
        Assert.Equal("PENDIENTE", result.Estado);
        Assert.Equal(10, result.AtencionID);
        Assert.Equal(10, result.PacienteID);
        Assert.Equal(5, result.MedicoID);
        Assert.Equal("NORMAL", Assert.Single(result.Detalle).Prioridad);

        var stored = Assert.Single(await solicitudes.GetAllAsync());
        Assert.Equal(result.NumeroSolicitud, stored.NumeroSolicitud);
        Assert.Single(stored.Detalle);
    }
}
