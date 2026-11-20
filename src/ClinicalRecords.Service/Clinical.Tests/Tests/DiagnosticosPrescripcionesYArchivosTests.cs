using Clinical.Core.Exceptions;
using Clinical.Core.Features.Clinical.Archivos.Commands.AdjuntarArchivoClinico;
using Clinical.Core.Features.Clinical.Diagnosticos.Commands.AsignarDiagnostico;
using Clinical.Core.Features.Clinical.Diagnosticos.Queries.GetDiagnosticos;
using Clinical.Core.Features.Clinical.Prescripciones.Commands.CreatePrescripcion;
using Clinical.Domain.Entities;
using Clinical.Tests.Fakes;

namespace Clinical.Tests.Tests;

public class DiagnosticosPrescripcionesYArchivosTests
{
    [Fact]
    public async Task AsignarDiagnostico_CuandoHayDosPrincipales_LanzaPrincipalDuplicado()
    {
        var (handler, _) = BuildDiagnosticoHandler();

        await handler.Handle(new AsignarDiagnosticoCommand
        {
            AtencionID = 10,
            DiagnosticoID = 1,
            Tipo = "PRINCIPAL"
        }, CancellationToken.None);

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(
            new AsignarDiagnosticoCommand { AtencionID = 10, DiagnosticoID = 2, Tipo = "PRINCIPAL" },
            CancellationToken.None));

        Assert.Equal(Errores.DIAGNOSTICO_PRINCIPAL_DUPLICADO, ex.Codigo);
    }

    [Fact]
    public async Task AsignarDiagnostico_CuandoEstaInactivo_LanzaDiagnosticoNoEncontrado()
    {
        var diagnosticos = new FakeDiagnosticoRepository();
        diagnosticos.Seed(new Diagnostico { DiagnosticoID = 1, Codigo = "I10", Nombre = "Hipertensión", Activo = false });
        var atenciones = new FakeAtencionRepository();
        atenciones.Seed(new Atencion { AtencionID = 10 });
        var handler = new AsignarDiagnosticoCommandHandler(
            new FakeAtencionDiagnosticoRepository(), atenciones, diagnosticos);

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(
            new AsignarDiagnosticoCommand { AtencionID = 10, DiagnosticoID = 1, Tipo = "PRINCIPAL" },
            CancellationToken.None));

        Assert.Equal(Errores.DIAGNOSTICO_NO_ENCONTRADO, ex.Codigo);
    }

    [Fact]
    public async Task CreatePrescripcion_CuandoNoHayDetalle_LanzaPrescripcionSinDetalle()
    {
        var atenciones = new FakeAtencionRepository();
        atenciones.Seed(new Atencion { AtencionID = 10 });
        var handler = new CreatePrescripcionCommandHandler(
            new FakePrescripcionRepository(), atenciones, new FakeMedicamentoRepository());

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(
            new CreatePrescripcionCommand { AtencionID = 10, MedicoID = 3 },
            CancellationToken.None));

        Assert.Equal(Errores.PRESCRIPCION_SIN_DETALLE, ex.Codigo);
    }

    [Fact]
    public async Task CreatePrescripcion_CuandoElMedicamentoEstaInactivo_LanzaMedicamentoNoValido()
    {
        var atenciones = new FakeAtencionRepository();
        atenciones.Seed(new Atencion { AtencionID = 10 });
        var medicamentos = new FakeMedicamentoRepository();
        medicamentos.Seed(new Medicamento { MedicamentoID = 5, Codigo = "MED-5", Nombre = "Paracetamol", Activo = false });
        var handler = new CreatePrescripcionCommandHandler(
            new FakePrescripcionRepository(), atenciones, medicamentos);

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(new CreatePrescripcionCommand
        {
            AtencionID = 10,
            MedicoID = 3,
            Detalle = new List<CreatePrescripcionDetalle>
            {
                new() { MedicamentoID = 5, Dosis = "500 mg", Frecuencia = "c/8h", Via = "Oral" }
            }
        }, CancellationToken.None));

        Assert.Equal(Errores.MEDICAMENTO_NO_VALIDO, ex.Codigo);
    }

    [Fact]
    public async Task AdjuntarArchivo_CuandoNoExisteExpediente_LanzaExpedienteNoEncontrado()
    {
        var handler = new AdjuntarArchivoClinicoCommandHandler(
            new FakeArchivoClinicoRepository(),
            new FakeExpedienteClinicoRepository(),
            new FakeAtencionRepository());

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(
            new AdjuntarArchivoClinicoCommand
            {
                ExpedienteClinicoID = 99,
                NombreArchivo = "consentimiento.pdf",
                Ruta = "/archivos/consentimiento.pdf"
            },
            CancellationToken.None));

        Assert.Equal(Errores.EXPEDIENTE_NO_ENCONTRADO, ex.Codigo);
    }

    [Fact]
    public async Task GetDiagnosticos_BuscarPorCodigoYActivo_DevuelveSoloCoincidencias()
    {
        var repo = new FakeDiagnosticoRepository();
        repo.Seed(
            new Diagnostico { DiagnosticoID = 1, Codigo = "I10", Nombre = "Hipertensión", Activo = true },
            new Diagnostico { DiagnosticoID = 2, Codigo = "I10", Nombre = "Hipertensión crónica", Activo = false },
            new Diagnostico { DiagnosticoID = 3, Codigo = "E11", Nombre = "Diabetes", Activo = true });
        var handler = new GetDiagnosticosQueryHandler(repo);

        var result = await handler.Handle(new GetDiagnosticosQuery
        {
            Buscar = "I10",
            Activo = true
        }, CancellationToken.None);

        Assert.Equal(1, result.TotalRecords);
        Assert.Equal(1, Assert.Single(result.Data).DiagnosticoID);
    }

    private static (AsignarDiagnosticoCommandHandler handler, FakeAtencionDiagnosticoRepository repository) BuildDiagnosticoHandler()
    {
        var repository = new FakeAtencionDiagnosticoRepository();
        var atenciones = new FakeAtencionRepository();
        atenciones.Seed(new Atencion { AtencionID = 10, Estado = "EN_CURSO" });

        var diagnosticos = new FakeDiagnosticoRepository();
        diagnosticos.Seed(
            new Diagnostico { DiagnosticoID = 1, Codigo = "I10", Nombre = "Hipertensión", Activo = true },
            new Diagnostico { DiagnosticoID = 2, Codigo = "E11", Nombre = "Diabetes", Activo = true });

        return (new AsignarDiagnosticoCommandHandler(repository, atenciones, diagnosticos), repository);
    }
}
