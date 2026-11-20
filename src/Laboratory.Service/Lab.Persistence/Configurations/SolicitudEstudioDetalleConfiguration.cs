using Lab.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lab.Persistence.Configurations;

public class SolicitudEstudioDetalleConfiguration : IEntityTypeConfiguration<SolicitudEstudioDetalle>
{
    public void Configure(EntityTypeBuilder<SolicitudEstudioDetalle> builder)
    {
        builder.ToTable("Solicitud_Estudio_Detalle");

        builder.HasKey(d => d.SolicitudEstudioDetalleID);

        builder.Property(d => d.SolicitudEstudioDetalleID)
            .ValueGeneratedOnAdd();

        builder.Property(d => d.SolicitudEstudioID)
            .IsRequired();

        builder.Property(d => d.EstudioID)
            .IsRequired();

        builder.Property(d => d.Prioridad)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(d => d.Observaciones)
            .HasMaxLength(500);

        builder.Property(d => d.CreatedAt)
            .IsRequired();

        builder.HasOne(d => d.SolicitudEstudio)
            .WithMany(s => s.Detalle)
            .HasForeignKey(d => d.SolicitudEstudioID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Estudio)
            .WithMany()
            .HasForeignKey(d => d.EstudioID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
