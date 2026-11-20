using Clinical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinical.Persistence.Configurations;

public class SintomaConfiguration : IEntityTypeConfiguration<Sintoma>
{
    public void Configure(EntityTypeBuilder<Sintoma> builder)
    {
        builder.ToTable("Síntomas");
        builder.HasKey(s => s.SintomaID);
        builder.Property(s => s.SintomaID).ValueGeneratedOnAdd();
        builder.Property(s => s.Codigo).HasMaxLength(30).IsRequired();
        builder.HasIndex(s => s.Codigo).IsUnique();
        builder.Property(s => s.Nombre).HasMaxLength(200).IsRequired();
        builder.Property(s => s.Descripcion).HasMaxLength(500);
        builder.Property(s => s.Activo).IsRequired();
        builder.Property(s => s.CreatedAt).IsRequired();
    }
}
