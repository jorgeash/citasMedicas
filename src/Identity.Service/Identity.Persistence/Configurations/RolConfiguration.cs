using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Persistence.Configurations;

public class RolConfiguration : IEntityTypeConfiguration<Rol>
{
    public void Configure(EntityTypeBuilder<Rol> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(r => r.RolID);

        builder.Property(r => r.RolID)
            .HasColumnName("RolID")
            .ValueGeneratedOnAdd();

        builder.Property(r => r.Nombre).HasColumnName("Nombre").HasMaxLength(200).IsRequired();
        builder.Property(r => r.Descripcion).HasColumnName("Descripcion").HasMaxLength(600);
        builder.Property(r => r.Activo).HasColumnName("Activo").IsRequired();

        // UQ_Roles_Nombre (nombre de rol único).
        builder.HasIndex(r => r.Nombre).IsUnique().HasDatabaseName("UQ_Roles_Nombre");

        // Relación muchos-a-muchos con la tabla puente Rol_Permiso,
        // cuya PK es compuesta (RolID, PermisoID).
        //
        // OJO: las expresiones `e => e.Permiso` / `e => e.Rol` son
        // obligatorias: con `HasOne<T>()` (sin la navegación) EF descubre
        // después las propiedades de navegación por convención y crea una
        // SEGUNDA relación con FK sombra "PermisoID1", una columna inexistente
        // que rompe toda consulta sobre Rol_Permiso (login devolvía 500).
        builder.HasMany(r => r.Permisos)
            .WithMany()
            .UsingEntity<RolPermiso>(
                izq => izq.HasOne<Permiso>(e => e.Permiso)
                    .WithMany()
                    .HasForeignKey(e => e.PermisoID)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Rol_Permiso_Permiso"),
                der => der.HasOne<Rol>(e => e.Rol)
                    .WithMany()
                    .HasForeignKey(e => e.RolID)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Rol_Permiso_Rol"),
                puente =>
                {
                    puente.HasKey(x => new { x.RolID, x.PermisoID });
                    puente.ToTable("Rol_Permiso");
                });
    }
}
