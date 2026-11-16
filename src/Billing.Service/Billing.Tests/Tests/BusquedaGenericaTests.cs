using Billing.Core.Features.Billing.Facturas.Queries.GetFacturas;
using Billing.Core.Features.Billing.Servicios.Queries.GetServicios;
using Billing.Core.Features.Billing.Tarifas.Queries.GetTarifas;
using Billing.Domain.Entities;
using Billing.Domain.Enums;
using Billing.Tests.Fakes;
using Shared.Kernel.Helpers;

namespace Billing.Tests.Tests;

/// <summary>
/// Búsqueda genérica de texto libre (`buscar`): Filter.FreeText en los handlers de Billing.
/// Ojo: los repos en memoria usan LINQ-to-Objects, donde string.Contains SÍ distingue
/// mayúsculas (en SQL Server la colación lo hace insensible), por eso los datos de
/// prueba se buscan con la caja exacta.
/// </summary>
public class BusquedaGenericaTests
{
    /* -------------------------------------------------- Handler: Servicios */

    [Fact]
    public async Task GetServicios_BuscarPorCodigoONombre_DevuelveCoincidencias()
    {
        var repo = new FakeServicioRepository();
        repo.Seed(
            new Servicio { ServicioID = 1, Codigo = "CONSULTA", Nombre = "Consulta general" },
            new Servicio { ServicioID = 2, Codigo = "LABSANG", Nombre = "Laboratorio de sangre" });

        var handler = new GetServiciosQueryEventHandler(repo);

        var porCodigo = await handler.Handle(
            new GetServiciosQuery { Buscar = "LABSANG" }, CancellationToken.None);
        Assert.Equal(2, Assert.Single(porCodigo.Data.Data).ServicioID);

        var porNombre = await handler.Handle(
            new GetServiciosQuery { Buscar = "Consulta" }, CancellationToken.None);
        Assert.Equal(1, Assert.Single(porNombre.Data.Data).ServicioID);
    }

    [Fact]
    public async Task GetServicios_SinCoincidencias_DevuelveVacio()
    {
        var repo = new FakeServicioRepository();
        repo.Seed(new Servicio { ServicioID = 1, Codigo = "CONSULTA", Nombre = "Consulta general" });

        var handler = new GetServiciosQueryEventHandler(repo);
        var result = await handler.Handle(
            new GetServiciosQuery { Buscar = "NO-EXISTE" }, CancellationToken.None);

        Assert.Empty(result.Data.Data);
        Assert.Equal(0, result.Data.TotalRecords);
    }

    /* ---------------------------------------------------- Handler: Tarifas */

    [Fact]
    public async Task GetTarifas_BuscarPorNombreDeTarifa_YPorServicioNavegado()
    {
        var repo = new FakeTarifaRepository();
        repo.Seed(
            new Tarifa
            {
                TarifaID = 1,
                ServicioID = 1,
                NombreTarifa = "Tarifa particular",
                Servicio = new Servicio { ServicioID = 1, Codigo = "CONSULTA", Nombre = "Consulta general" }
            },
            new Tarifa
            {
                TarifaID = 2,
                ServicioID = 2,
                NombreTarifa = "Tarifa convenio",
                Servicio = new Servicio { ServicioID = 2, Codigo = "LABSANG", Nombre = "Laboratorio de sangre" }
            });

        var handler = new GetTarifasQueryEventHandler(repo);

        // Campo propio de la tarifa.
        var porTarifa = await handler.Handle(
            new GetTarifasQuery { Buscar = "particular" }, CancellationToken.None);
        Assert.Equal(1, Assert.Single(porTarifa.Data.Data).TarifaID);

        // Ruta anidada Servicio.Nombre.
        var porServicio = await handler.Handle(
            new GetTarifasQuery { Buscar = "Laboratorio" }, CancellationToken.None);
        Assert.Equal(2, Assert.Single(porServicio.Data.Data).TarifaID);
    }

    /* --------------------------------------------------- Handler: Facturas */

    [Fact]
    public async Task GetFacturas_BuscarPorNumeroOPaciente()
    {
        var repo = new FakeFacturaRepository();
        repo.Seed(
            new Factura
            {
                FacturaID = 1,
                Numero = "FAC-000001",
                PacienteID = 1,
                PacienteNombre = "Juan Pérez",
                Estado = EstadoFactura.PENDIENTE
            },
            new Factura
            {
                FacturaID = 2,
                Numero = "FAC-000002",
                PacienteID = 2,
                PacienteNombre = "María López",
                Estado = EstadoFactura.PAGADA
            });

        var handler = new GetFacturasQueryHandler(repo);

        var porNumero = await handler.Handle(
            new GetFacturasQuery { Buscar = "FAC-000002" }, CancellationToken.None);
        Assert.Equal(2, Assert.Single(porNumero.Data).FacturaID);

        var porPaciente = await handler.Handle(
            new GetFacturasQuery { Buscar = "Juan" }, CancellationToken.None);
        Assert.Equal(1, Assert.Single(porPaciente.Data).FacturaID);
    }

    [Fact]
    public async Task GetFacturas_BuscarSeCombinaConElEstadoEnAND()
    {
        var repo = new FakeFacturaRepository();
        repo.Seed(
            new Factura
            {
                FacturaID = 1,
                Numero = "FAC-000001",
                PacienteID = 1,
                PacienteNombre = "Juan Pérez",
                Estado = EstadoFactura.PENDIENTE
            },
            new Factura
            {
                FacturaID = 2,
                Numero = "FAC-000002",
                PacienteID = 2,
                PacienteNombre = "Juan Delgado",
                Estado = EstadoFactura.PAGADA
            });

        var handler = new GetFacturasQueryHandler(repo);
        var result = await handler.Handle(new GetFacturasQuery
        {
            Buscar = "Juan",                      // coincide en ambas
            Estado = EstadoFactura.PAGADA         // pero solo la pagada
        }, CancellationToken.None);

        Assert.Equal(1, result.TotalRecords);
        Assert.Equal(2, Assert.Single(result.Data).FacturaID);
    }

    [Fact]
    public async Task GetFacturas_SinCoincidencias_DevuelveVacio()
    {
        var repo = new FakeFacturaRepository();
        repo.Seed(new Factura
        {
            FacturaID = 1,
            Numero = "FAC-000001",
            PacienteID = 1,
            PacienteNombre = "Juan Pérez",
            Estado = EstadoFactura.PENDIENTE
        });

        var handler = new GetFacturasQueryHandler(repo);
        var result = await handler.Handle(
            new GetFacturasQuery { Buscar = "no-figura" }, CancellationToken.None);

        Assert.Empty(result.Data);
        Assert.Equal(0, result.TotalRecords);
    }
}
