using Billing.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Billing.Persistence.Context;

public class BillingDbContext : DbContext
{
    public BillingDbContext(DbContextOptions<BillingDbContext> options) : base(options) { }

    public DbSet<Servicio> Servicios => Set<Servicio>();
    public DbSet<Tarifa> Tarifas => Set<Tarifa>();
    public DbSet<Factura> Facturas => Set<Factura>();
    public DbSet<FacturaDetalle> FacturaDetalles => Set<FacturaDetalle>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BillingDbContext).Assembly);
    }
}
