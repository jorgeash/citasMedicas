using Billing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Billing.Persistence.Configurations;

public class FacturaDetalleConfiguration : IEntityTypeConfiguration<FacturaDetalle>
{
    public void Configure(EntityTypeBuilder<FacturaDetalle> builder)
    {
        builder.ToTable("Factura_Detalle");

        builder.HasKey(d => d.FacturaDetalleID);

        builder.Property(d => d.FacturaDetalleID)
            .ValueGeneratedOnAdd();

        builder.Property(d => d.Descripcion)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(d => d.Cantidad)
            .IsRequired();

        builder.Property(d => d.PrecioUnitario)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(d => d.Subtotal)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasOne(d => d.Servicio)
            .WithMany()
            .HasForeignKey(d => d.ServicioID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Tarifa)
            .WithMany()
            .HasForeignKey(d => d.TarifaID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
