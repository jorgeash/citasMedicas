using Addresses.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Addresses.Persistence.Configurations;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    /// <summary>
    /// Catalogo de paises. Los datos de HasData son los datos de prueba iniciales: viajan
    /// dentro de la migracion, asi que se aplican de forma idempotente con "database update".
    /// </summary>
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.ToTable("Addresses_Country");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id).ValueGeneratedOnAdd();

        builder.Property(c => c.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(c => c.Name).IsUnique();

        builder.HasData(
            new Country { Id = 1, Name = "Ecuador", IsActive = true, CreatedAt = SeedDefaults.CreatedAt },
            new Country { Id = 2, Name = "Colombia", IsActive = true, CreatedAt = SeedDefaults.CreatedAt }
        );
    }
}