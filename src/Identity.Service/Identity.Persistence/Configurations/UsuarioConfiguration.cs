using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Persistence.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");

        builder.HasKey(u => u.UsuarioID);

        builder.Property(u => u.UsuarioID)
            .HasColumnName("UsuarioID")
            .ValueGeneratedOnAdd();

        builder.Property(u => u.RolID).HasColumnName("RolID").IsRequired();
        builder.Property(u => u.SucursalID).HasColumnName("SucursalID");
        builder.Property(u => u.NombreUsuario).HasColumnName("NombreUsuario").HasMaxLength(100).IsRequired();
        builder.Property(u => u.NombreCompleto).HasColumnName("NombreCompleto").HasMaxLength(400).IsRequired();
        builder.Property(u => u.Email).HasColumnName("Email").HasMaxLength(150);
        builder.Property(u => u.PasswordHash).HasColumnName("PasswordHash").HasMaxLength(500).IsRequired();
        builder.Property(u => u.Activo).HasColumnName("Activo").IsRequired();
        builder.Property(u => u.UltimoAcceso).HasColumnName("UltimoAcceso").HasColumnType("datetime2");
        builder.Property(u => u.FechaCreacion).HasColumnName("FechaCreacion").HasColumnType("datetime2").IsRequired();

        // UQ_Usuarios_Nombre (nombre de usuario único).
        builder.HasIndex(u => u.NombreUsuario).IsUnique().HasDatabaseName("UQ_Usuarios_Nombre");

        builder.HasOne(u => u.Rol)
            .WithMany(r => r.Usuarios)
            .HasForeignKey(u => u.RolID)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Usuarios_Roles");
    }
}
