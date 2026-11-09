using Billing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Billing.Persistence.Configurations;

public class FacturaConfiguration : IEntityTypeConfiguration<Factura>
{
    public void Configure(EntityTypeBuilder<Factura> builder)
    {
        builder.ToTable("Facturas");

        builder.HasKey(f => f.FacturaID);

        builder.Property(f => f.FacturaID)
            .ValueGeneratedOnAdd();

        builder.Property(f => f.Numero)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(f => f.Numero)
            .IsUnique();

        builder.Property(f => f.PacienteID)
            .IsRequired();

        builder.Property(f => f.PacienteNombre)
            .HasMaxLength(200);

        builder.Property(f => f.CitaID);

        builder.Property(f => f.AseguradoraID);

        builder.Property(f => f.FechaEmision)
            .IsRequired();

        builder.Property(f => f.Subtotal)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(f => f.Impuesto)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(f => f.Total)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(f => f.Saldo)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(f => f.Moneda)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(f => f.Estado)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(f => f.Observaciones)
            .HasMaxLength(500);

        builder.Property(f => f.CreatedAt)
            .IsRequired();

        builder.Property(f => f.UpdatedAt);

        builder.HasMany(f => f.Detalle)
            .WithOne(d => d.Factura)
            .HasForeignKey(d => d.FacturaID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
