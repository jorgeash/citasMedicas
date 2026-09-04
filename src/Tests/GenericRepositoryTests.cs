using System.Threading.Tasks;
using Xunit;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Persistence.Billing.Context;
using Persistence.Billing.Repositories;
using Domain.Billing.Entities;

namespace Tests.Persistence.Tests;

public class GenericRepositoryTests
{
    private BillingDbContext CreateContext(SqliteConnection connection)
    {
        var options = new DbContextOptionsBuilder<BillingDbContext>()
            .UseSqlite(connection)
            .Options;

        return new BillingDbContext(options);
    }

    [Fact]
    public async Task Paciente_CRUD_Works()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        using var context = CreateContext(connection);
        context.Database.EnsureCreated();

        var repo = new PacienteRepository(context);

        var paciente = new Paciente { Nombres = "Juan", Apellidos = "Perez", FechaNacimiento = DateTime.UtcNow.AddYears(-30) };

        var created = await repo.CreateAsync(paciente);
        Assert.NotNull(created);
        Assert.True(created.PacienteID > 0);

        var got = await repo.GetByIdAsync(created.PacienteID);
        Assert.NotNull(got);
        Assert.Equal("Juan", got!.Nombres);

        got.Apellidos = "Perez Lopez";
        var updated = await repo.UpdateAsync(got);
        Assert.Equal("Perez Lopez", updated.Apellidos);

        var deleted = await repo.DeleteAsync(updated);
        Assert.True(deleted);

        var after = await repo.GetByIdAsync(created.PacienteID);
        Assert.Null(after);
    }

    [Fact]
    public async Task Tarifa_CRUD_Works()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        using var context = CreateContext(connection);
        context.Database.EnsureCreated();

        var servicioRepo = new ServicioRepository(context);
        var tarifaRepo = new TarifaRepository(context);

        var servicio = new Servicio { Codigo = "S002", Nombre = "Examen", TipoServicio = "Laboratorio", Activo = true };
        servicio = await servicioRepo.CreateAsync(servicio);

        var tarifa = new Tarifa { ServicioID = servicio.ServicioID, NombreTarifa = "Basica", Precio = 10m, FechaInicio = DateOnly.FromDateTime(DateTime.UtcNow), Activa = true };
        var created = await tarifaRepo.CreateAsync(tarifa);
        Assert.NotNull(created);
        Assert.True(created.TarifaID > 0);

        var got = await tarifaRepo.GetByIdAsync(created.TarifaID);
        Assert.NotNull(got);
        Assert.Equal("Basica", got!.NombreTarifa);

        var byServicio = await tarifaRepo.GetByServicioIdAsync(servicio.ServicioID);
        Assert.Contains(byServicio, t => t.TarifaID == created.TarifaID);

        // Update
        got.NombreTarifa = "Basica Plus";
        var updated = await tarifaRepo.UpdateAsync(got);
        Assert.Equal("Basica Plus", updated.NombreTarifa);

        // Delete
        var deleted = await tarifaRepo.DeleteAsync(updated);
        Assert.True(deleted);
    }

    [Fact]
    public async Task Servicio_CRUD_Works()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        using var context = CreateContext(connection);
        context.Database.EnsureCreated();

        var repo = new ServicioRepository(context);

        var servicio = new Servicio { Codigo = "S001", Nombre = "Consulta", TipoServicio = "General", Activo = true };

        // Create
        var created = await repo.CreateAsync(servicio);
        Assert.NotNull(created);
        Assert.True(created.ServicioID > 0);

        // GetById
        var got = await repo.GetByIdAsync(created.ServicioID);
        Assert.NotNull(got);
        Assert.Equal("S001", got!.Codigo);

        // GetAll
        var all = await repo.GetAllAsync();
        Assert.Contains(all, s => s.ServicioID == created.ServicioID);

        // Update
        got!.Nombre = "Consulta General";
        var updated = await repo.UpdateAsync(got);
        Assert.Equal("Consulta General", updated.Nombre);

        // Exists
        var exists = await repo.ExistsAsync(s => s.Codigo == "S001");
        Assert.True(exists);

        // Count
        var count = await repo.CountAsync();
        Assert.True(count >= 1);

        // Delete
        var deleted = await repo.DeleteAsync(updated);
        Assert.True(deleted);

        var after = await repo.GetByIdAsync(created.ServicioID);
        Assert.Null(after);
    }
}
