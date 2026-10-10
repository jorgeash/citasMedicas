using Business.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Business.Persistence.Configurations;

public class CustomerTypeConfiguration : IEntityTypeConfiguration<CustomerType>
{
    public void Configure(EntityTypeBuilder<CustomerType> builder)
    {
        builder.ToTable("Business_CustomerType");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id).ValueGeneratedOnAdd();

        builder.Property(t => t.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(t => t.Name).IsUnique();

        // Datos de prueba del catalogo de tipos de cliente.
        builder.HasData(
            new CustomerType { Id = 1, Name = "Empresa", IsActive = true, CreatedAt = SeedDefaults.CreatedAt },
            new CustomerType { Id = 2, Name = "Persona Natural", IsActive = true, CreatedAt = SeedDefaults.CreatedAt },
            new CustomerType { Id = 3, Name = "EPS / Aseguradora", IsActive = true, CreatedAt = SeedDefaults.CreatedAt }
        );
    }
}