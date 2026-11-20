using Clinical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinical.Persistence.Configurations;

public class TratamientoConfiguration : IEntityTypeConfiguration<Tratamiento>
{
    public void Configure(EntityTypeBuilder<Tratamiento> builder)
    {
        builder.ToTable("Tratamientos");
        builder.HasKey(t => t.TratamientoID);
        builder.Property(t => t.TratamientoID).ValueGeneratedOnAdd();
        builder.Property(t => t.AtencionID).IsRequired();
        builder.Property(t => t.Nombre).HasMaxLength(200).IsRequired();
        builder.Property(t => t.Objetivo).HasMaxLength(500);
        builder.Property(t => t.FechaInicio).IsRequired();
        builder.Property(t => t.FechaFin);
        builder.Property(t => t.Estado).HasMaxLength(30).IsRequired();
        builder.Property(t => t.Observaciones).HasMaxLength(500);
        builder.Property(t => t.CreatedAt).IsRequired();
        builder.HasOne(t => t.Atencion)
            .WithMany(a => a.Tratamientos)
            .HasForeignKey(t => t.AtencionID)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(t => t.Detalle)
            .WithOne(d => d.Tratamiento)
            .HasForeignKey(d => d.TratamientoID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
