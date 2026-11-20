using Clinical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinical.Persistence.Configurations;

public class ExpedienteClinicoConfiguration : IEntityTypeConfiguration<ExpedienteClinico>
{
    public void Configure(EntityTypeBuilder<ExpedienteClinico> builder)
    {
        builder.ToTable("Expedientes_Clínicos");
        builder.HasKey(e => e.ExpedienteClinicoID);
        builder.Property(e => e.ExpedienteClinicoID).ValueGeneratedOnAdd();
        builder.Property(e => e.PacienteID).IsRequired();
        builder.Property(e => e.NumeroExpediente).HasMaxLength(20).IsRequired();
        builder.HasIndex(e => e.NumeroExpediente).IsUnique();
        builder.HasIndex(e => e.PacienteID);
        builder.Property(e => e.FechaApertura).IsRequired();
        builder.Property(e => e.Observaciones).HasMaxLength(500);
        builder.Property(e => e.Activo).IsRequired();
        builder.Property(e => e.CreatedAt).IsRequired();
        builder.Property(e => e.UpdatedAt);
    }
}
