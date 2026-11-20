using Lab.Core.Features.Lab.Estudios.Queries.GetEstudios;
using Lab.Core.Features.Lab.ResultadosEstudio.Queries.GetResultados;
using Lab.Core.Features.Lab.ResultadosEstudio.Queries.GetResultadosBySolicitud;
using Lab.Core.Features.Lab.SolicitudesEstudio.Queries.GetSolicitudesEstudio;
using Lab.Domain.Entities;
using Lab.Tests.Fakes;

namespace Lab.Tests.Tests;

public class BusquedaGenericaTests
{
    [Fact]
    public async Task GetEstudios_BusquedaLibreYPaginacion_DevuelveCoincidencias()
    {
        var repository = new FakeEstudioRepository();
        repository.Seed(
            new Estudio { EstudioID = 1, Codigo = "HEMO", Nombre = "Hemograma completo", Tipo = "LABORATORIO", Activo = true },
            new Estudio { EstudioID = 2, Codigo = "GLU", Nombre = "Glucosa", Tipo = "LABORATORIO", Activo = true },
            new Estudio { EstudioID = 3, Codigo = "RX", Nombre = "Radiografia", Tipo = "IMAGEN", Activo = true });
        var handler = new GetEstudiosQueryHandler(repository);

        var porNombre = await handler.Handle(new GetEstudiosQuery { Buscar = "Glucosa" }, CancellationToken.None);
        Assert.Equal(2, Assert.Single(porNombre.Data).EstudioID);

        var pagina = await handler.Handle(
            new GetEstudiosQuery { Activo = true, PageNumber = 2, PageSize = 2 },
            CancellationToken.None);
        Assert.Equal(3, pagina.TotalRecords);
        Assert.Equal(2, pagina.TotalPage);
        Assert.Equal(2, pagina.CurrentPage);
        Assert.Single(pagina.Data);
    }

    [Fact]
    public async Task GetEstudios_FiltroActivoYTipo_SeCombinaEnAnd()
    {
        var repository = new FakeEstudioRepository();
        repository.Seed(
            new Estudio { EstudioID = 1, Codigo = "HEMO", Nombre = "Hemograma", Tipo = "LABORATORIO", Activo = true },
            new Estudio { EstudioID = 2, Codigo = "RX", Nombre = "Radiografia", Tipo = "IMAGEN", Activo = false },
            new Estudio { EstudioID = 3, Codigo = "ECO", Nombre = "Ecografia", Tipo = "IMAGEN", Activo = true });
        var handler = new GetEstudiosQueryHandler(repository);

        var result = await handler.Handle(
            new GetEstudiosQuery { Activo = true, Tipo = "imagen" },
            CancellationToken.None);

        Assert.Equal(1, result.TotalRecords);
        Assert.Equal(3, Assert.Single(result.Data).EstudioID);
    }

    [Fact]
    public async Task GetSolicitudesEstudio_FiltrosYBuscar_IncluyeDetalleYResultados()
    {
        var repository = new FakeSolicitudEstudioRepository();
        var solicitudUno = new SolicitudEstudio
        {
            SolicitudEstudioID = 1,
            NumeroSolicitud = "SE-2026-000001",
            PacienteID = 10,
            Estado = "PENDIENTE",
            FechaSolicitud = new DateTime(2026, 1, 10)
        };
        solicitudUno.Detalle.Add(new SolicitudEstudioDetalle
        {
            SolicitudEstudioDetalleID = 11,
            SolicitudEstudioID = 1,
            EstudioID = 1
        });
        repository.Seed(
            solicitudUno,
            new SolicitudEstudio
            {
                SolicitudEstudioID = 2,
                NumeroSolicitud = "SE-2026-000002",
                PacienteID = 10,
                Estado = "COMPLETADA",
                FechaSolicitud = new DateTime(2026, 2, 10)
            },
            new SolicitudEstudio
            {
                SolicitudEstudioID = 3,
                NumeroSolicitud = "SE-2026-000003",
                PacienteID = 20,
                Estado = "PENDIENTE",
                FechaSolicitud = new DateTime(2026, 3, 10)
            });

        var handler = new GetSolicitudesEstudioQueryHandler(repository);

        var porFiltros = await handler.Handle(new GetSolicitudesEstudioQuery
        {
            PacienteID = 10,
            Estado = "PENDIENTE",
            Desde = new DateTime(2026, 1, 1),
            Hasta = new DateTime(2026, 1, 31)
        }, CancellationToken.None);

        Assert.Equal(1, porFiltros.TotalRecords);
        Assert.Equal(1, Assert.Single(porFiltros.Data).SolicitudEstudioID);
        Assert.Single(Assert.Single(porFiltros.Data).Detalle);

        var porNumero = await handler.Handle(new GetSolicitudesEstudioQuery
        {
            Buscar = "SE-2026-000002"
        }, CancellationToken.None);

        Assert.Equal(2, Assert.Single(porNumero.Data).SolicitudEstudioID);
    }

    [Fact]
    public async Task GetResultados_FiltroPacienteYDesde_DevuelveSoloLosDelPaciente()
    {
        var solicitudes = new FakeSolicitudEstudioRepository();
        var s1 = new SolicitudEstudio
        {
            SolicitudEstudioID = 1,
            NumeroSolicitud = "SE-2026-000001",
            PacienteID = 10
        };
        var s2 = new SolicitudEstudio
        {
            SolicitudEstudioID = 2,
            NumeroSolicitud = "SE-2026-000002",
            PacienteID = 20
        };
        solicitudes.Seed(s1, s2);

        var resultados = new FakeResultadoEstudioRepository();
        resultados.Seed(
            new ResultadoEstudio
            {
                ResultadoEstudioID = 1,
                SolicitudEstudioID = 1,
                SolicitudEstudioDetalleID = 11,
                FechaResultado = new DateTime(2026, 1, 10),
                Valor = "5.1",
                Estado = "NORMAL",
                SolicitudEstudio = s1
            },
            new ResultadoEstudio
            {
                ResultadoEstudioID = 2,
                SolicitudEstudioID = 2,
                SolicitudEstudioDetalleID = 21,
                FechaResultado = new DateTime(2026, 2, 10),
                Valor = "CRITICO",
                Estado = "CRITICO",
                SolicitudEstudio = s2
            },
            new ResultadoEstudio
            {
                ResultadoEstudioID = 3,
                SolicitudEstudioID = 1,
                SolicitudEstudioDetalleID = 12,
                FechaResultado = new DateTime(2026, 3, 10),
                Valor = "7.2",
                Estado = "ANORMAL",
                SolicitudEstudio = s1
            });

        var handler = new GetResultadosQueryHandler(resultados, solicitudes);

        var result = await handler.Handle(new GetResultadosQuery
        {
            PacienteID = 10,
            Desde = new DateTime(2026, 2, 1)
        }, CancellationToken.None);

        Assert.Equal(1, result.TotalRecords);
        var dto = Assert.Single(result.Data);
        Assert.Equal(3, dto.ResultadoEstudioID);
        Assert.Equal(10, dto.PacienteID);
        Assert.Equal("SE-2026-000001", dto.NumeroSolicitud);
    }

    [Fact]
    public async Task GetResultadosBySolicitud_DevuelveSoloLosDeEsaSolicitud()
    {
        var repository = new FakeResultadoEstudioRepository();
        repository.Seed(
            new ResultadoEstudio
            {
                ResultadoEstudioID = 1,
                SolicitudEstudioID = 1,
                SolicitudEstudioDetalleID = 11,
                FechaResultado = new DateTime(2026, 1, 10),
                Valor = "5.1",
                Estado = "NORMAL"
            },
            new ResultadoEstudio
            {
                ResultadoEstudioID = 2,
                SolicitudEstudioID = 2,
                SolicitudEstudioDetalleID = 21,
                FechaResultado = new DateTime(2026, 1, 11),
                Valor = "7.2",
                Estado = "NORMAL"
            });

        var handler = new GetResultadosBySolicitudQueryHandler(repository);

        var result = await handler.Handle(new GetResultadosBySolicitudQuery(1), CancellationToken.None);

        Assert.Equal(1, Assert.Single(result).ResultadoEstudioID);
    }
}
