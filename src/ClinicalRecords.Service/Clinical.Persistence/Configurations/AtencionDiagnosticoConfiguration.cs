using Clinical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinical.Persistence.Configurations;

public class AtencionDiagnosticoConfiguration : IEntityTypeConfiguration<AtencionDiagnostico>
{
    public void Configure(EntityTypeBuilder<AtencionDiagnostico> builder)
    {
        builder.ToTable("Atención_Diagnóstico");
        builder.HasKey(a => a.AtencionDiagnosticoID);
        builder.Property(a => a.AtencionDiagnosticoID).ValueGeneratedOnAdd();
        builder.Property(a => a.AtencionID).IsRequired();
        builder.Property(a => a.DiagnosticoID).IsRequired();
        builder.Property(a => a.Tipo).HasMaxLength(30).IsRequired();
        builder.Property(a => a.Observaciones).HasMaxLength(500);
        builder.Property(a => a.CreatedAt).IsRequired();
        builder.HasOne(a => a.Atencion)
            .WithMany(t => t.Diagnosticos)
            .HasForeignKey(a => a.AtencionID)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(a => a.Diagnostico)
            .WithMany(d => d.AtencionDiagnosticos)
            .HasForeignKey(a => a.DiagnosticoID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
