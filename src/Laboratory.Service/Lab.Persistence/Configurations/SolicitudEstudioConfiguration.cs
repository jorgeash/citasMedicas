using Lab.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lab.Persistence.Configurations;

public class SolicitudEstudioConfiguration : IEntityTypeConfiguration<SolicitudEstudio>
{
    public void Configure(EntityTypeBuilder<SolicitudEstudio> builder)
    {
        builder.ToTable("Solicitudes_Estudio");

        builder.HasKey(s => s.SolicitudEstudioID);

        builder.Property(s => s.SolicitudEstudioID)
            .ValueGeneratedOnAdd();

        builder.Property(s => s.NumeroSolicitud)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex(s => s.NumeroSolicitud)
            .IsUnique();

        builder.Property(s => s.AtencionID)
            .IsRequired();

        builder.Property(s => s.PacienteID)
            .IsRequired();

        builder.Property(s => s.MedicoID)
            .IsRequired();

        builder.Property(s => s.FechaSolicitud)
            .IsRequired();

        builder.Property(s => s.Estado)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(s => s.Observaciones)
            .HasMaxLength(500);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt);

        builder.HasMany(s => s.Detalle)
            .WithOne(d => d.SolicitudEstudio)
            .HasForeignKey(d => d.SolicitudEstudioID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.Resultados)
            .WithOne(r => r.SolicitudEstudio)
            .HasForeignKey(r => r.SolicitudEstudioID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
