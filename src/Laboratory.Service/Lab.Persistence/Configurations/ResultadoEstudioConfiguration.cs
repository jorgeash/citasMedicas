using Lab.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lab.Persistence.Configurations;

public class ResultadoEstudioConfiguration : IEntityTypeConfiguration<ResultadoEstudio>
{
    public void Configure(EntityTypeBuilder<ResultadoEstudio> builder)
    {
        builder.ToTable("Resultados_Estudio");

        builder.HasKey(r => r.ResultadoEstudioID);

        builder.Property(r => r.ResultadoEstudioID)
            .ValueGeneratedOnAdd();

        builder.Property(r => r.SolicitudEstudioID)
            .IsRequired();

        builder.Property(r => r.SolicitudEstudioDetalleID)
            .IsRequired();

        builder.Property(r => r.FechaResultado)
            .IsRequired();

        builder.Property(r => r.Valor)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(r => r.Unidad)
            .HasMaxLength(50);

        builder.Property(r => r.RangoReferencia)
            .HasMaxLength(200);

        builder.Property(r => r.Estado)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(r => r.Interpretacion)
            .HasMaxLength(2000);

        builder.Property(r => r.ArchivoUrl)
            .HasMaxLength(500);

        builder.Property(r => r.ValidadoPorMedicoID);

        builder.Property(r => r.FechaValidacion);

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.HasOne(r => r.SolicitudEstudio)
            .WithMany(s => s.Resultados)
            .HasForeignKey(r => r.SolicitudEstudioID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.SolicitudEstudioDetalle)
            .WithMany()
            .HasForeignKey(r => r.SolicitudEstudioDetalleID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
