using Clinical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinical.Persistence.Configurations;

public class AtencionSintomaConfiguration : IEntityTypeConfiguration<AtencionSintoma>
{
    public void Configure(EntityTypeBuilder<AtencionSintoma> builder)
    {
        builder.ToTable("Atención_Síntoma");
        builder.HasKey(a => a.AtencionSintomaID);
        builder.Property(a => a.AtencionSintomaID).ValueGeneratedOnAdd();
        builder.Property(a => a.AtencionID).IsRequired();
        builder.Property(a => a.SintomaID).IsRequired();
        builder.Property(a => a.Severidad).HasMaxLength(30).IsRequired();
        builder.Property(a => a.DuracionDias);
        builder.Property(a => a.Observaciones).HasMaxLength(500);
        builder.Property(a => a.CreatedAt).IsRequired();
        builder.HasOne(a => a.Atencion)
            .WithMany(t => t.Sintomas)
            .HasForeignKey(a => a.AtencionID)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(a => a.Sintoma)
            .WithMany(s => s.AtencionSintomas)
            .HasForeignKey(a => a.SintomaID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
