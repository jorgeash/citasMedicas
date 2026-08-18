using Domain.Billing.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Billing.Context;

public class BillingDbContext : DbContext
{
    public BillingDbContext(DbContextOptions<BillingDbContext> options) : base(options) { }

    public DbSet<Servicio> Servicios => Set<Servicio>();
    public DbSet<Tarifa> Tarifas => Set<Tarifa>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BillingDbContext).Assembly);
    }
}
