using Clinical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinical.Persistence.Configurations;

public class PrescripcionConfiguration : IEntityTypeConfiguration<Prescripcion>
{
    public void Configure(EntityTypeBuilder<Prescripcion> builder)
    {
        builder.ToTable("Prescripciones");
        builder.HasKey(p => p.PrescripcionID);
        builder.Property(p => p.PrescripcionID).ValueGeneratedOnAdd();
        builder.Property(p => p.AtencionID).IsRequired();
        builder.Property(p => p.MedicoID).IsRequired();
        builder.Property(p => p.FechaPrescripcion).IsRequired();
        builder.Property(p => p.FechaInicio);
        builder.Property(p => p.FechaFin);
        builder.Property(p => p.Estado).HasMaxLength(30).IsRequired();
        builder.Property(p => p.Observaciones).HasMaxLength(500);
        builder.Property(p => p.CreatedAt).IsRequired();
        builder.HasOne(p => p.Atencion)
            .WithMany(a => a.Prescripciones)
            .HasForeignKey(p => p.AtencionID)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(p => p.Detalle)
            .WithOne(d => d.Prescripcion)
            .HasForeignKey(d => d.PrescripcionID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
