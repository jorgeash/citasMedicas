using Clinical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinical.Persistence.Configurations;

public class HabitoConfiguration : IEntityTypeConfiguration<Habito>
{
    public void Configure(EntityTypeBuilder<Habito> builder)
    {
        builder.ToTable("Hábitos");
        builder.HasKey(h => h.HabitoID);
        builder.Property(h => h.HabitoID).ValueGeneratedOnAdd();
        builder.Property(h => h.ExpedienteClinicoID).IsRequired();
        builder.Property(h => h.Fuma).IsRequired();
        builder.Property(h => h.ConsumeAlcohol).IsRequired();
        builder.Property(h => h.ActividadFisica).IsRequired();
        builder.Property(h => h.FrecuenciaActividad).HasMaxLength(100);
        builder.Property(h => h.Observaciones).HasMaxLength(500);
        builder.Property(h => h.CreatedAt).IsRequired();
        builder.HasOne(h => h.ExpedienteClinico)
            .WithMany(e => e.Habitos)
            .HasForeignKey(h => h.ExpedienteClinicoID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
