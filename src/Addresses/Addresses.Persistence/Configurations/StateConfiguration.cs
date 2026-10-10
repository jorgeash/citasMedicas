using Addresses.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Addresses.Persistence.Configurations;

public class StateConfiguration : IEntityTypeConfiguration<State>
{
    public void Configure(EntityTypeBuilder<State> builder)
    {
        builder.ToTable("Addresses_State");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id).ValueGeneratedOnAdd();

        builder.Property(s => s.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasOne(s => s.Country)
            .WithMany(c => c.States)
            .HasForeignKey(s => s.CountryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(s => new { s.CountryId, s.Name }).IsUnique();

        builder.HasData(
            new State { Id = 1, Name = "Pichincha", CountryId = 1, IsActive = true, CreatedAt = SeedDefaults.CreatedAt },
            new State { Id = 2, Name = "Guayas", CountryId = 1, IsActive = true, CreatedAt = SeedDefaults.CreatedAt },
            new State { Id = 3, Name = "Cundinamarca", CountryId = 2, IsActive = true, CreatedAt = SeedDefaults.CreatedAt }
        );
    }
}