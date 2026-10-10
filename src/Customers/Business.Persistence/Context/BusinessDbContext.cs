using Business.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Business.Persistence.Context;

/// <summary>
/// DbContext del microservicio de clientes. Las tablas conservan los nombres del microservicio
/// original (Business_*) para que el mapeo con el codigo heredado sea directo.
/// </summary>
public class BusinessDbContext : DbContext
{
    public BusinessDbContext(DbContextOptions<BusinessDbContext> options) : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<CustomerType> CustomerTypes => Set<CustomerType>();
    public DbSet<CustomerContact> CustomerContacts => Set<CustomerContact>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BusinessDbContext).Assembly);
    }
}