using Clinical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinical.Persistence.Configurations;

public class AlergiaConfiguration : IEntityTypeConfiguration<Alergia>
{
    public void Configure(EntityTypeBuilder<Alergia> builder)
    {
        builder.ToTable("Alergias");
        builder.HasKey(a => a.AlergiaID);
        builder.Property(a => a.AlergiaID).ValueGeneratedOnAdd();
        builder.Property(a => a.ExpedienteClinicoID).IsRequired();
        builder.Property(a => a.Sustancia).HasMaxLength(200).IsRequired();
        builder.Property(a => a.Tipo).HasMaxLength(50).IsRequired();
        builder.Property(a => a.Severidad).HasMaxLength(30).IsRequired();
        builder.Property(a => a.Reaccion).HasMaxLength(500);
        builder.Property(a => a.FechaDeteccion);
        builder.Property(a => a.Activo).IsRequired();
        builder.Property(a => a.CreatedAt).IsRequired();
        builder.HasOne(a => a.ExpedienteClinico)
            .WithMany(e => e.Alergias)
            .HasForeignKey(a => a.ExpedienteClinicoID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
