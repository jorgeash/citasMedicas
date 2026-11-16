using System.ComponentModel.DataAnnotations;
using Scheduling.Core.Features.Scheduling.Aseguradoras.Queries.GetAseguradoras;
using Scheduling.Core.Features.Scheduling.Citas.Queries.GetCitas;
using Scheduling.Core.Features.Scheduling.Medicos.Queries.GetMedicos;
using Scheduling.Core.Features.Scheduling.Pacientes.Queries.GetPacientes;
using Scheduling.Domain.Entities;
using Scheduling.Tests.Fakes;
using Shared.Kernel.Helpers;

namespace Scheduling.Tests.Tests;

/// <summary>
/// Búsqueda genérica de texto libre (`buscar`): Filter.FreeText + su uso en los handlers.
/// Ojo: los repos en memoria usan LINQ-to-Objects, donde string.Contains SÍ distingue
/// mayúsculas (en SQL Server la colación lo hace insensible), por eso los datos de
/// prueba se buscan con la caja exacta.
/// </summary>
public class BusquedaGenericaTests
{
    /* -------------------------------------------------- Filter.FreeText */

    [Fact]
    public void FreeText_BuscaEnCualquieraDeLosCamposIndicados()
    {
        var filtro = Filter.FreeText<Paciente>("Juan", "Nombres", "Apellidos").Compile();

        var juan = new Paciente { PacienteID = 1, Nombres = "Juan", Apellidos = "Pérez" };
        var perez = new Paciente { PacienteID = 2, Nombres = "Otro", Apellidos = "Pérez" };
        var maria = new Paciente { PacienteID = 3, Nombres = "María", Apellidos = "López" };

        Assert.True(filtro(juan));   // coincide en Nombres
        Assert.False(filtro(perez)); // "Juan" no está en sus campos
        Assert.False(filtro(maria));
    }

    [Fact]
    public void FreeText_SoportaVariosCamposEnOR()
    {
        // "López" está en Apellidos del segundo, "María" en Nombres del primero.
        var filtro = Filter.FreeText<Paciente>("López", "Nombres", "Apellidos").Compile();

        var maria = new Paciente { PacienteID = 1, Nombres = "María", Apellidos = "López" };
        var otro = new Paciente { PacienteID = 2, Nombres = "Carlos", Apellidos = "Ruiz" };

        Assert.True(filtro(maria));
        Assert.False(filtro(otro));
    }

    [Fact]
    public void FreeText_ConRutaAnizada_BuscaEnLaEntidadNavegada()
    {
        var filtro = Filter.FreeText<Cita>("Juan", "Motivo", "Paciente.Nombres").Compile();

        var cita = new Cita
        {
            CitaID = 1,
            PacienteID = 1,
            Motivo = "Control anual",
            Paciente = new Paciente { PacienteID = 1, Nombres = "Juan", Apellidos = "Pérez" }
        };

        Assert.True(filtro(cita)); // coincide por Paciente.Nombres
    }

    [Fact]
    public void FreeText_ConNavigationNula_NoLanzaYNoCoincide()
    {
        var filtro = Filter.FreeText<Cita>("Juan", "Motivo", "Paciente.Nombres").Compile();

        var citaSinPaciente = new Cita
        {
            CitaID = 2,
            PacienteID = 9,
            Motivo = "Dolor de cabeza",
            Paciente = null
        };

        // La guarda de null evita NullReferenceException y simplemente no coincide.
        Assert.False(filtro(citaSinPaciente));
    }

    [Fact]
    public void FreeText_SinCoincidencias_DevuelveFalse()
    {
        var filtro = Filter.FreeText<Paciente>("zzz-no-existe", "Nombres", "Apellidos").Compile();

        Assert.False(filtro(new Paciente { PacienteID = 1, Nombres = "Juan", Apellidos = "Pérez" }));
    }

    [Fact]
    public void FreeText_TextoVacio_NoAplicaFiltro()
    {
        var filtro = Filter.FreeText<Paciente>("   ", "Nombres").Compile();

        Assert.True(filtro(new Paciente { PacienteID = 1, Nombres = "Juan", Apellidos = "Pérez" }));
    }

    [Fact]
    public void FreeText_CampoInexistente_LanzaValidationException()
    {
        Assert.Throws<ValidationException>(
            () => Filter.FreeText<Paciente>("algo", "CampoQueNoExiste"));
    }

    [Fact]
    public void FreeText_CampoNoString_LanzaValidationException()
    {
        Assert.Throws<ValidationException>(
            () => Filter.FreeText<Paciente>("algo", "FechaNacimiento"));
    }

    [Fact]
    public void FreeText_SinRutas_LanzaValidationException()
    {
        Assert.Throws<ValidationException>(() => Filter.FreeText<Paciente>("algo"));
    }

    /* ------------------------------------------------- Handler: Pacientes */

    [Fact]
    public async Task GetPacientes_BuscarPorNombre_DevuelveSoloCoincidencias()
    {
        var repo = new FakePacienteRepository();
        repo.Seed(
            new Paciente { PacienteID = 1, CodigoPaciente = "PAC-000001", Nombres = "Juan", Apellidos = "Pérez" },
            new Paciente { PacienteID = 2, CodigoPaciente = "PAC-000002", Nombres = "María", Apellidos = "López" });

        var handler = new GetPacientesQueryHandler(repo);
        var result = await handler.Handle(
            new GetPacientesQuery { Buscar = "María" }, CancellationToken.None);

        var único = Assert.Single(result.Data.Data);
        Assert.Equal(2, único.PacienteID);
        Assert.Equal(1, result.Data.TotalRecords);
    }

    [Fact]
    public async Task GetPacientes_BuscarPorCodigo_SinCoincidencias_DevuelveVacio()
    {
        var repo = new FakePacienteRepository();
        repo.Seed(new Paciente { PacienteID = 1, CodigoPaciente = "PAC-000001", Nombres = "Juan", Apellidos = "Pérez" });

        var handler = new GetPacientesQueryHandler(repo);
        var result = await handler.Handle(
            new GetPacientesQuery { Buscar = "PAC-999" }, CancellationToken.None);

        Assert.Empty(result.Data.Data);
        Assert.Equal(0, result.Data.TotalRecords);
    }

    [Fact]
    public async Task GetPacientes_BuscarSeCombinaConElFilterEnAND()
    {
        var repo = new FakePacienteRepository();
        repo.Seed(
            new Paciente { PacienteID = 1, Nombres = "Juan", Apellidos = "Pérez", Sexo = "M" },
            new Paciente { PacienteID = 2, Nombres = "María", Apellidos = "López", Sexo = "F" },
            new Paciente { PacienteID = 3, Nombres = "Lucía", Apellidos = "López", Sexo = "F" });

        var handler = new GetPacientesQueryHandler(repo);
        var result = await handler.Handle(new GetPacientesQuery
        {
            // filter = expresión LINQ potencia-usuario; buscar = texto libre.
            Filter = "Sexo == \"F\"",
            Buscar = "María"
        }, CancellationToken.None);

        // AND: solo María (F y coincide "María"); Lucía es F pero no coincide.
        var único = Assert.Single(result.Data.Data);
        Assert.Equal(2, único.PacienteID);
    }

    /* ---------------------------------------------------- Handler: Citas */

    [Fact]
    public async Task GetCitas_BuscarPorNombreDePaciente_FiltraPorLaNavegacion()
    {
        var repo = new FakeCitaRepository();
        repo.Seed(
            new Cita
            {
                CitaID = 1,
                PacienteID = 1,
                EstadoCitaID = 1,
                FechaHoraInicio = new DateTime(2026, 11, 20, 9, 0, 0),
                Motivo = "Consulta de control",
                Paciente = new Paciente { PacienteID = 1, Nombres = "Juan", Apellidos = "Pérez" },
                Medico = new Medico { MedicoID = 1, Nombres = "Ana", Apellidos = "Gómez" }
            },
            new Cita
            {
                CitaID = 2,
                PacienteID = 2,
                EstadoCitaID = 1,
                FechaHoraInicio = new DateTime(2026, 11, 21, 10, 0, 0),
                Motivo = "Chequeo",
                Paciente = new Paciente { PacienteID = 2, Nombres = "María", Apellidos = "López" },
                Medico = new Medico { MedicoID = 1, Nombres = "Ana", Apellidos = "Gómez" }
            });

        var handler = new GetCitasQueryHandler(repo);
        var result = await handler.Handle(
            new GetCitasQuery { Buscar = "Juan" }, CancellationToken.None);

        Assert.Equal(1, result.Data.TotalRecords);
        Assert.Equal(1, Assert.Single(result.Data.Data).CitaID);
    }

    [Fact]
    public async Task GetCitas_BuscarPorMotivo_YCombinadoConEstado()
    {
        var repo = new FakeCitaRepository();
        repo.Seed(
            new Cita
            {
                CitaID = 1,
                PacienteID = 1,
                EstadoCitaID = 1,
                FechaHoraInicio = new DateTime(2026, 11, 20, 9, 0, 0),
                Motivo = "Dolor de cabeza",
                Paciente = new Paciente { PacienteID = 1, Nombres = "Juan", Apellidos = "Pérez" }
            },
            new Cita
            {
                CitaID = 2,
                PacienteID = 2,
                EstadoCitaID = 2,
                FechaHoraInicio = new DateTime(2026, 11, 10, 10, 0, 0),
                Motivo = "Dolor de cabeza",
                Paciente = new Paciente { PacienteID = 2, Nombres = "María", Apellidos = "López" }
            });

        var handler = new GetCitasQueryHandler(repo);
        var result = await handler.Handle(new GetCitasQuery
        {
            Buscar = "Dolor",   // coincide en ambas
            EstadoCitaID = 2    // pero solo la atendida
        }, CancellationToken.None);

        Assert.Equal(1, result.Data.TotalRecords);
        Assert.Equal(2, Assert.Single(result.Data.Data).CitaID);
    }

    /* -------------------------------------------------- Handler: Médicos */

    [Fact]
    public async Task GetMedicos_BuscarPorCodigoOApellido()
    {
        var repo = new FakeMedicoRepository();
        repo.Seed(
            new Medico { MedicoID = 1, CodigoMedico = "MED-001", Nombres = "Ana María", Apellidos = "Gómez" },
            new Medico { MedicoID = 2, CodigoMedico = "MED-002", Nombres = "Luis", Apellidos = "Castro" });

        var handler = new GetMedicosQueryHandler(repo);

        var porCodigo = await handler.Handle(
            new GetMedicosQuery { Buscar = "MED-002" }, CancellationToken.None);
        Assert.Equal(2, Assert.Single(porCodigo.Data.Data).MedicoID);

        var porApellido = await handler.Handle(
            new GetMedicosQuery { Buscar = "Gómez" }, CancellationToken.None);
        Assert.Equal(1, Assert.Single(porApellido.Data.Data).MedicoID);
    }

    /* --------------------------------------------- Handler: Aseguradoras */

    [Fact]
    public async Task GetAseguradoras_BuscarPorNombre()
    {
        var repo = new FakeAseguradoraRepository();
        repo.Seed(
            new Aseguradora { AseguradoraID = 1, Codigo = "SEGSALUD", Nombre = "Seguros Salud S.A." },
            new Aseguradora { AseguradoraID = 2, Codigo = "PARTICULAR", Nombre = "Particular (sin seguro)" });

        var handler = new GetAseguradorasQueryHandler(repo);
        var result = await handler.Handle(
            new GetAseguradorasQuery { Buscar = "Salud" }, CancellationToken.None);

        var única = Assert.Single(result.Data.Data);
        Assert.Equal(1, única.AseguradoraID);
    }
}
