using Clinical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinical.Persistence.Configurations;

public class AntecedenteMedicoConfiguration : IEntityTypeConfiguration<AntecedenteMedico>
{
    public void Configure(EntityTypeBuilder<AntecedenteMedico> builder)
    {
        builder.ToTable("Antecedentes_Médicos");
        builder.HasKey(a => a.AntecedenteMedicoID);
        builder.Property(a => a.AntecedenteMedicoID).ValueGeneratedOnAdd();
        builder.Property(a => a.ExpedienteClinicoID).IsRequired();
        builder.Property(a => a.Tipo).HasMaxLength(50).IsRequired();
        builder.Property(a => a.Descripcion).HasMaxLength(2000).IsRequired();
        builder.Property(a => a.Fecha);
        builder.Property(a => a.Observaciones).HasMaxLength(500);
        builder.Property(a => a.Activo).IsRequired();
        builder.Property(a => a.CreatedAt).IsRequired();
        builder.HasOne(a => a.ExpedienteClinico)
            .WithMany(e => e.AntecedentesMedicos)
            .HasForeignKey(a => a.ExpedienteClinicoID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
