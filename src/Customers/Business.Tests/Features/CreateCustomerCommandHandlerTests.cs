using Business.Core.DTOs.External;
using Business.Core.Exceptions;
using Business.Core.Features.Customers;
using Business.Core.Interfaces.Repositories;
using Business.Core.Interfaces.Services;
using Business.Persistence.Context;
using Business.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Business.Tests.Features;

/// <summary>
/// Pruebas del alta de clientes con EF Core InMemory: cubren la validacion del LocationId
/// contra el catalogo de direcciones, que es la integracion entre los dos microservicios.
/// </summary>
public class CreateCustomerCommandHandlerTests : IDisposable
{
    private readonly BusinessDbContext _contexto;

    public CreateCustomerCommandHandlerTests()
    {
        var opciones = new DbContextOptionsBuilder<BusinessDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _contexto = new BusinessDbContext(opciones);
        _contexto.Database.EnsureCreated();
    }

    public void Dispose() => _contexto.Dispose();

    [Fact]
    public async Task Handle_LocationIdValido_PersisteElClienteConLaDireccionResuelta()
    {
        var ubicaciones = new FakeLocationService(new LocationDto
        {
            Id = 3,
            Address = "Calle 100 # 15-30, Oficina 501",
            ZipCode = "EC170150",
            CityId = 2,
            CityName = "Guayaquil",
            CountryId = 1,
            CountryName = "Ecuador",
            Latitude = -2.1894,
            Longitude = -79.8891,
            IsActive = true
        });

        var handler = new CreateCustomerCommandHandler(
            new CustomerRepository(_contexto),
            new CustomerTypeRepository(_contexto),
            new CompanyRepository(_contexto),
            ubicaciones);

        var response = await handler.Handle(new CreateCustomerCommand
        {
            Code = "CLI-0002",
            Name = "Clinica Vida Salud S.A.",
            Email = "facturacion@clinicavidasalud.example",
            CustomerTypeId = 1,
            CompanyId = 1,
            LocationId = 3
        }, CancellationToken.None);

        Assert.True(response.Succeeded);
        Assert.Equal(3, response.Data.LocationId);
        Assert.Equal("Guayaquil", response.Data.Location?.CityName);
        Assert.Equal("Calle 100 # 15-30, Oficina 501", response.Data.Location?.Address);
        Assert.Equal(1, ubicaciones.Llamadas);

        var persistido = await new CustomerRepository(_contexto).GetOneByAsync(c => c.Code == "CLI-0002");
        Assert.NotNull(persistido);
        Assert.Equal(3, persistido!.LocationId);
    }

    [Fact]
    public async Task Handle_LocationIdNoExistente_LanzaDomainExceptionYNoPersiste()
    {
        var handler = new CreateCustomerCommandHandler(
            new CustomerRepository(_contexto),
            new CustomerTypeRepository(_contexto),
            new CompanyRepository(_contexto),
            new FakeLocationService(null));

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(new CreateCustomerCommand
        {
            Code = "CLI-0003",
            Name = "EPS Total Salud S.A.",
            CustomerTypeId = 3,
            CompanyId = 3,
            LocationId = 999
        }, CancellationToken.None));

        Assert.Equal(Errores.LOCATION_NO_ENCONTRADA, ex.Codigo);

        // La base ya trae el cliente de ejemplo de la migracion (CLI-0001); lo que no debe
        // existir es el cliente que se intento crear.
        Assert.Null(await new CustomerRepository(_contexto).GetOneByAsync(c => c.Code == "CLI-0003"));
    }

    [Fact]
    public async Task Handle_LocationIdCero_LanzaDomainExceptionAntesDeConsultarElOtroServicio()
    {
        var ubicaciones = new FakeLocationService(new LocationDto { Id = 1 });
        var handler = new CreateCustomerCommandHandler(
            new CustomerRepository(_contexto),
            new CustomerTypeRepository(_contexto),
            new CompanyRepository(_contexto),
            ubicaciones);

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(new CreateCustomerCommand
        {
            Code = "CLI-0004",
            Name = "Empresa sin direccion",
            CustomerTypeId = 1,
            CompanyId = 1,
            LocationId = 0
        }, CancellationToken.None));

        Assert.Equal(Errores.LOCATION_ID_INVALIDO, ex.Codigo);
        Assert.Equal(0, ubicaciones.Llamadas);
    }

    [Fact]
    public async Task Handle_CodigoDuplicado_LanzaDomainException()
    {
        var handler = new CreateCustomerCommandHandler(
            new CustomerRepository(_contexto),
            new CustomerTypeRepository(_contexto),
            new CompanyRepository(_contexto),
            new FakeLocationService(new LocationDto { Id = 1, Address = "Av. Amazonas N34-1234" }));

        var comando = new CreateCustomerCommand
        {
            Code = "CLI-9001",
            Name = "Clinica Central del Sur",
            CustomerTypeId = 1,
            CompanyId = 1,
            LocationId = 1
        };

        await handler.Handle(comando, CancellationToken.None);

        var ex = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(comando, CancellationToken.None));

        Assert.Equal(Errores.CODIGO_CLIENTE_DUPLICADO, ex.Codigo);
    }

    private sealed class FakeLocationService : ILocationService
    {
        private readonly LocationDto? _location;

        public FakeLocationService(LocationDto? location) => _location = location;

        public int Llamadas { get; private set; }

        public Task<LocationDto?> ObtenerLocationAsync(int locationId, CancellationToken cancellationToken = default)
        {
            Llamadas++;
            return Task.FromResult(_location?.Id == locationId ? _location : null);
        }

        public Task<IReadOnlyList<LocationDto>> ListarLocationsAsync(int pageSize = 50, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<LocationDto>>(
                _location is null ? Array.Empty<LocationDto>() : new[] { _location });
    }
}