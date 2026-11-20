using Clinical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinical.Persistence.Configurations;

public class ProcedimientoConfiguration : IEntityTypeConfiguration<Procedimiento>
{
    public void Configure(EntityTypeBuilder<Procedimiento> builder)
    {
        builder.ToTable("Procedimientos");
        builder.HasKey(p => p.ProcedimientoID);
        builder.Property(p => p.ProcedimientoID).ValueGeneratedOnAdd();
        builder.Property(p => p.Codigo).HasMaxLength(30).IsRequired();
        builder.HasIndex(p => p.Codigo).IsUnique();
        builder.Property(p => p.Nombre).HasMaxLength(200).IsRequired();
        builder.Property(p => p.Descripcion).HasMaxLength(500);
        builder.Property(p => p.Activo).IsRequired();
        builder.Property(p => p.CreatedAt).IsRequired();
    }
}
