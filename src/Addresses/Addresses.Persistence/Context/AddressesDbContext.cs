using Addresses.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Addresses.Persistence.Context;

/// <summary>
/// DbContext del microservicio de direcciones. Las tablas conservan los nombres del
/// microservicio original (Addresses_*) para que el mapeo con el codigo heredado sea directo.
/// </summary>
public class AddressesDbContext : DbContext
{
    public AddressesDbContext(DbContextOptions<AddressesDbContext> options) : base(options) { }

    public DbSet<Country> Countries => Set<Country>();
    public DbSet<State> States => Set<State>();
    public DbSet<City> Cities => Set<City>();
    public DbSet<Position> Positions => Set<Position>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AddressesDbContext).Assembly);
    }
}