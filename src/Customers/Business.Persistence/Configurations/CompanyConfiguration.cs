using Business.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Business.Persistence.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Business_Company");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id).ValueGeneratedOnAdd();

        builder.Property(c => c.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Sigla)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(c => c.MainEmail)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(c => c.Sigla).IsUnique();

        // Datos de prueba del catalogo de empresas.
        builder.HasData(
            new Company
            {
                Id = 1,
                Name = "Clinica Vida Salud S.A.",
                Sigla = "CVS",
                MainEmail = "contacto@clinicavidasalud.example",
                IsActive = true,
                CreatedAt = SeedDefaults.CreatedAt
            },
            new Company
            {
                Id = 2,
                Name = "Distribuidora Andina CIA LTDA",
                Sigla = "DAN",
                MainEmail = "compras@distribuidoraandina.example",
                IsActive = true,
                CreatedAt = SeedDefaults.CreatedAt
            },
            new Company
            {
                Id = 3,
                Name = "EPS-total Salud S.A.",
                Sigla = "EPST",
                MainEmail = "servicios.cliente@epstotal.example",
                IsActive = true,
                CreatedAt = SeedDefaults.CreatedAt
            }
        );
    }
}