using Clinical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinical.Persistence.Configurations;

public class AtencionConfiguration : IEntityTypeConfiguration<Atencion>
{
    public void Configure(EntityTypeBuilder<Atencion> builder)
    {
        builder.ToTable("Atenciones");
        builder.HasKey(a => a.AtencionID);
        builder.Property(a => a.AtencionID).ValueGeneratedOnAdd();
        builder.Property(a => a.CitaID).IsRequired();
        builder.HasIndex(a => a.CitaID).IsUnique();
        builder.Property(a => a.PacienteID).IsRequired();
        builder.Property(a => a.ExpedienteClinicoID);
        builder.Property(a => a.MedicoID).IsRequired();
        builder.Property(a => a.FechaAtencion).IsRequired();
        builder.Property(a => a.Motivo).HasMaxLength(500);
        builder.Property(a => a.Antecedente).HasMaxLength(2000);
        builder.Property(a => a.ExamenFisico).HasMaxLength(2000);
        builder.Property(a => a.Conclusion).HasMaxLength(2000);
        builder.Property(a => a.Estado).HasMaxLength(30).IsRequired();
        builder.Property(a => a.CreatedAt).IsRequired();
        builder.Property(a => a.UpdatedAt);
        builder.HasOne(a => a.ExpedienteClinico)
            .WithMany(e => e.Atenciones)
            .HasForeignKey(a => a.ExpedienteClinicoID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
