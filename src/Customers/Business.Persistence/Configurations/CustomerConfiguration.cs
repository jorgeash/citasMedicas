using Business.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Business.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    /// <summary>
    /// Business_Customer es la tabla "Customers" de la actividad. LocationId es un entero
    /// sin restriccion FK: apunta a Addresses_Position, que vive en el microservicio de
    /// Addresses, por eso la validacion se hace por HTTP con la libreria NugetPackage_Rest.
    /// La fila de HasData es el cliente de prueba previo (codigo CLI-0001) que ya existia
    /// antes de crear los clientes nuevos por la API.
    /// </summary>
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Business_Customer");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id).ValueGeneratedOnAdd();

        builder.Property(c => c.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Email)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.LocationId)
            .IsRequired();

        builder.HasIndex(c => c.Code).IsUnique();

        builder.HasIndex(c => c.LocationId);

        builder.HasOne(c => c.CustomerType)
            .WithMany(t => t.Customers)
            .HasForeignKey(c => c.CustomerTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Company)
            .WithMany(e => e.Customers)
            .HasForeignKey(c => c.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Dato de prueba: cliente ya registrado que apunta a la direccion 2 del catalogo
        // de Addresses (Av. Francisco de Orellana EO7-22, Quito).
        builder.HasData(
            new Customer
            {
                Id = 1,
                Code = "CLI-0001",
                Name = "Distribuidora Andina CIA LTDA",
                Email = "compras@distribuidoraandina.example",
                CustomerTypeId = 1,
                CompanyId = 2,
                LocationId = 2,
                IsActive = true,
                CreatedAt = SeedDefaults.CreatedAt
            }
        );
    }
}