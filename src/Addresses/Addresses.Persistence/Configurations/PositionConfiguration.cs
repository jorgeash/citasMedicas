using Addresses.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Addresses.Persistence.Configurations;

public class PositionConfiguration : IEntityTypeConfiguration<Position>
{
    /// <summary>
    /// Addresses_Position es la tabla "Address" de la actividad: guarda las direcciones
    /// fisicas. Sus filas son las que se referencian desde Customers con el campo LocationId.
    /// Las cuatro filas de HasData son los datos de prueba del catalogo de direcciones.
    /// </summary>
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        builder.ToTable("Addresses_Position");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).ValueGeneratedOnAdd();

        builder.Property(p => p.Address)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(p => p.ZipCode)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(p => p.Latitude).IsRequired();
        builder.Property(p => p.Longitude).IsRequired();

        builder.HasOne(p => p.City)
            .WithMany(c => c.Locations)
            .HasForeignKey(p => p.CityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new Position
            {
                Id = 1,
                Address = "Av. Amazonas N34-1234, Edificio Clinica Norte",
                ZipCode = "EC010101",
                CityId = 1,
                Latitude = -0.18070,
                Longitude = -78.46780,
                CustomerId = null,
                IsActive = true,
                CreatedAt = SeedDefaults.CreatedAt
            },
            new Position
            {
                Id = 2,
                Address = "Av. Francisco de Orellana EO7-22, Piso 3",
                ZipCode = "EC010201",
                CityId = 1,
                Latitude = -0.17000,
                Longitude = -78.45000,
                CustomerId = null,
                IsActive = true,
                CreatedAt = SeedDefaults.CreatedAt
            },
            new Position
            {
                Id = 3,
                Address = "Av. 9 de Octubre y Los Rios, Torre Norte",
                ZipCode = "EC090100",
                CityId = 2,
                Latitude = -2.18940,
                Longitude = -79.88910,
                CustomerId = null,
                IsActive = true,
                CreatedAt = SeedDefaults.CreatedAt
            },
            new Position
            {
                Id = 4,
                Address = "Calle 100 # 11-60, Local 204",
                ZipCode = "CO110111",
                CityId = 3,
                Latitude = 4.71100,
                Longitude = -74.07210,
                CustomerId = null,
                IsActive = true,
                CreatedAt = SeedDefaults.CreatedAt
            }
        );
    }
}