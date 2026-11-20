using Clinical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinical.Persistence.Configurations;

public class HospitalizacionConfiguration : IEntityTypeConfiguration<Hospitalizacion>
{
    public void Configure(EntityTypeBuilder<Hospitalizacion> builder)
    {
        builder.ToTable("Hospitalizaciones");
        builder.HasKey(h => h.HospitalizacionID);
        builder.Property(h => h.HospitalizacionID).ValueGeneratedOnAdd();
        builder.Property(h => h.ExpedienteClinicoID).IsRequired();
        builder.Property(h => h.AtencionID).IsRequired();
        builder.Property(h => h.CamaID).IsRequired();
        builder.Property(h => h.FechaIngreso).IsRequired();
        builder.Property(h => h.FechaEgreso);
        builder.Property(h => h.Motivo).HasMaxLength(1000).IsRequired();
        builder.Property(h => h.Diagnostico).HasMaxLength(500);
        builder.Property(h => h.Estado).HasMaxLength(30).IsRequired();
        builder.Property(h => h.Observaciones).HasMaxLength(500);
        builder.Property(h => h.CreatedAt).IsRequired();
        builder.Property(h => h.UpdatedAt);
        builder.HasOne(h => h.ExpedienteClinico)
            .WithMany(e => e.Hospitalizaciones)
            .HasForeignKey(h => h.ExpedienteClinicoID)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(h => h.Atencion)
            .WithMany(a => a.Hospitalizaciones)
            .HasForeignKey(h => h.AtencionID)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(h => h.Cama)
            .WithMany(c => c.Hospitalizaciones)
            .HasForeignKey(h => h.CamaID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
