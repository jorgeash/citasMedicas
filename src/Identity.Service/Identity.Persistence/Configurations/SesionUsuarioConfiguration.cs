using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Persistence.Configurations;

public class SesionUsuarioConfiguration : IEntityTypeConfiguration<SesionUsuario>
{
    public void Configure(EntityTypeBuilder<SesionUsuario> builder)
    {
        builder.ToTable("Sesiones_Usuario");

        builder.HasKey(s => s.SesionID);

        builder.Property(s => s.SesionID)
            .HasColumnName("SesionID")
            .ValueGeneratedOnAdd();

        builder.Property(s => s.UsuarioID).HasColumnName("UsuarioID").IsRequired();
        builder.Property(s => s.TokenSesion).HasColumnName("TokenSesion").HasMaxLength(500);
        builder.Property(s => s.FechaInicio).HasColumnName("FechaInicio").HasColumnType("datetime2").IsRequired();
        builder.Property(s => s.FechaFin).HasColumnName("FechaFin").HasColumnType("datetime2");
        builder.Property(s => s.DireccionIP).HasColumnName("DireccionIP").HasMaxLength(50);
        builder.Property(s => s.Dispositivo).HasColumnName("Dispositivo").HasMaxLength(600);
        builder.Property(s => s.Activa).HasColumnName("Activa").IsRequired();

        builder.HasOne(s => s.Usuario)
            .WithMany()
            .HasForeignKey(s => s.UsuarioID)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Sesiones_Usuarios");
    }
}
