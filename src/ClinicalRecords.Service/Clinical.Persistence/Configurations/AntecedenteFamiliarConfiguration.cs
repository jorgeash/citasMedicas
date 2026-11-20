using Clinical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinical.Persistence.Configurations;

public class AntecedenteFamiliarConfiguration : IEntityTypeConfiguration<AntecedenteFamiliar>
{
    public void Configure(EntityTypeBuilder<AntecedenteFamiliar> builder)
    {
        builder.ToTable("Antecedentes_Familiares");
        builder.HasKey(a => a.AntecedenteFamiliarID);
        builder.Property(a => a.AntecedenteFamiliarID).ValueGeneratedOnAdd();
        builder.Property(a => a.ExpedienteClinicoID).IsRequired();
        builder.Property(a => a.Parentesco).HasMaxLength(50).IsRequired();
        builder.Property(a => a.Enfermedad).HasMaxLength(200).IsRequired();
        builder.Property(a => a.Observaciones).HasMaxLength(500);
        builder.Property(a => a.Activo).IsRequired();
        builder.Property(a => a.CreatedAt).IsRequired();
        builder.HasOne(a => a.ExpedienteClinico)
            .WithMany(e => e.AntecedentesFamiliares)
            .HasForeignKey(a => a.ExpedienteClinicoID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
