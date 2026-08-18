using Domain.Billing.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Billing.Configurations;

public class TarifaConfiguration : IEntityTypeConfiguration<Tarifa>
{
    public void Configure(EntityTypeBuilder<Tarifa> builder)
    {
        builder.ToTable("Tarifas");

        builder.HasKey(t => t.TarifaID);

        builder.Property(t => t.TarifaID)
            .ValueGeneratedOnAdd();

        builder.Property(t => t.NombreTarifa)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(t => t.Precio)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(t => t.Moneda)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(t => t.FechaInicio)
            .IsRequired();

        builder.Property(t => t.FechaFin);

        builder.Property(t => t.Activa)
            .IsRequired();

        builder.HasOne(t => t.Servicio)
            .WithMany()
            .HasForeignKey(t => t.ServicioID);

        builder.Property(t => t.AseguradoraID);
    }
}
