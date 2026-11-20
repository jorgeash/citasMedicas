using Clinical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinical.Persistence.Configurations;

public class PrescripcionDetalleConfiguration : IEntityTypeConfiguration<PrescripcionDetalle>
{
    public void Configure(EntityTypeBuilder<PrescripcionDetalle> builder)
    {
        builder.ToTable("Prescripción_Detalle");
        builder.HasKey(d => d.PrescripcionDetalleID);
        builder.Property(d => d.PrescripcionDetalleID).ValueGeneratedOnAdd();
        builder.Property(d => d.PrescripcionID).IsRequired();
        builder.Property(d => d.MedicamentoID).IsRequired();
        builder.Property(d => d.Dosis).HasMaxLength(100).IsRequired();
        builder.Property(d => d.Frecuencia).HasMaxLength(100).IsRequired();
        builder.Property(d => d.Via).HasMaxLength(50).IsRequired();
        builder.Property(d => d.DuracionDias);
        builder.Property(d => d.Cantidad);
        builder.Property(d => d.Observaciones).HasMaxLength(500);
        builder.Property(d => d.CreatedAt).IsRequired();
        builder.HasOne(d => d.Medicamento)
            .WithMany(m => m.PrescripcionDetalles)
            .HasForeignKey(d => d.MedicamentoID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
