using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Persistence.Configurations;

public class PermisoConfiguration : IEntityTypeConfiguration<Permiso>
{
    public void Configure(EntityTypeBuilder<Permiso> builder)
    {
        builder.ToTable("Permisos");

        builder.HasKey(p => p.PermisoID);

        builder.Property(p => p.PermisoID)
            .HasColumnName("PermisoID")
            .ValueGeneratedOnAdd();

        builder.Property(p => p.Codigo).HasColumnName("Codigo").HasMaxLength(100).IsRequired();
        builder.Property(p => p.Nombre).HasColumnName("Nombre").HasMaxLength(300).IsRequired();
        builder.Property(p => p.Descripcion).HasColumnName("Descripcion").HasMaxLength(600);

        // UQ_Permisos_Codigo (código de permiso único).
        builder.HasIndex(p => p.Codigo).IsUnique().HasDatabaseName("UQ_Permisos_Codigo");
    }
}
