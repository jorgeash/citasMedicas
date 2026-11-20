using Clinical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinical.Persistence.Configurations;

public class DiagnosticoConfiguration : IEntityTypeConfiguration<Diagnostico>
{
    public void Configure(EntityTypeBuilder<Diagnostico> builder)
    {
        builder.ToTable("Diagnósticos");
        builder.HasKey(d => d.DiagnosticoID);
        builder.Property(d => d.DiagnosticoID).ValueGeneratedOnAdd();
        builder.Property(d => d.Codigo).HasMaxLength(30).IsRequired();
        builder.HasIndex(d => d.Codigo).IsUnique();
        builder.Property(d => d.Nombre).HasMaxLength(200).IsRequired();
        builder.Property(d => d.Descripcion).HasMaxLength(500);
        builder.Property(d => d.Activo).IsRequired();
        builder.Property(d => d.CreatedAt).IsRequired();
    }
}
