using Clinical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinical.Persistence.Configurations;

public class IndicacionMedicaConfiguration : IEntityTypeConfiguration<IndicacionMedica>
{
    public void Configure(EntityTypeBuilder<IndicacionMedica> builder)
    {
        builder.ToTable("Indicaciones_Médicas");
        builder.HasKey(i => i.IndicacionMedicaID);
        builder.Property(i => i.IndicacionMedicaID).ValueGeneratedOnAdd();
        builder.Property(i => i.AtencionID).IsRequired();
        builder.Property(i => i.MedicoID).IsRequired();
        builder.Property(i => i.Tipo).HasMaxLength(50).IsRequired();
        builder.Property(i => i.Descripcion).HasMaxLength(2000).IsRequired();
        builder.Property(i => i.Activa).IsRequired();
        builder.Property(i => i.CreatedAt).IsRequired();
        builder.HasOne(i => i.Atencion)
            .WithMany(a => a.IndicacionesMedicas)
            .HasForeignKey(i => i.AtencionID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
