using Clinical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinical.Persistence.Configurations;

public class ArchivoClinicoConfiguration : IEntityTypeConfiguration<ArchivoClinico>
{
    public void Configure(EntityTypeBuilder<ArchivoClinico> builder)
    {
        builder.ToTable("Archivos_Clínicos");
        builder.HasKey(a => a.ArchivoClinicoID);
        builder.Property(a => a.ArchivoClinicoID).ValueGeneratedOnAdd();
        builder.Property(a => a.ExpedienteClinicoID).IsRequired();
        builder.Property(a => a.AtencionID);
        builder.Property(a => a.Tipo).HasMaxLength(50).IsRequired();
        builder.Property(a => a.NombreArchivo).HasMaxLength(300).IsRequired();
        builder.Property(a => a.Ruta).HasMaxLength(1000).IsRequired();
        builder.Property(a => a.ContentType).HasMaxLength(100);
        builder.Property(a => a.TamanoBytes);
        builder.Property(a => a.FechaCarga).IsRequired();
        builder.Property(a => a.Activo).IsRequired();
        builder.Property(a => a.CreatedAt).IsRequired();
        builder.HasOne(a => a.ExpedienteClinico)
            .WithMany(e => e.ArchivosClinicos)
            .HasForeignKey(a => a.ExpedienteClinicoID)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(a => a.Atencion)
            .WithMany(t => t.ArchivosClinicos)
            .HasForeignKey(a => a.AtencionID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
