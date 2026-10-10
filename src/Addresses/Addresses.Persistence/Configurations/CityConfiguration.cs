using Addresses.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Addresses.Persistence.Configurations;

public class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.ToTable("Addresses_City");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id).ValueGeneratedOnAdd();

        builder.Property(c => c.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasOne(c => c.Country)
            .WithMany(c => c.Cities)
            .HasForeignKey(c => c.CountryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => new { c.CountryId, c.Name }).IsUnique();

        builder.HasData(
            new City { Id = 1, Name = "Quito", CountryId = 1, IsActive = true, CreatedAt = SeedDefaults.CreatedAt },
            new City { Id = 2, Name = "Guayaquil", CountryId = 1, IsActive = true, CreatedAt = SeedDefaults.CreatedAt },
            new City { Id = 3, Name = "Bogota", CountryId = 2, IsActive = true, CreatedAt = SeedDefaults.CreatedAt }
        );
    }
}