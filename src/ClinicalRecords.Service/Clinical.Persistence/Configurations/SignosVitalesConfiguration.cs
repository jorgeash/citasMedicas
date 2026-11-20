using Clinical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinical.Persistence.Configurations;

public class SignosVitalesConfiguration : IEntityTypeConfiguration<SignosVitales>
{
    public void Configure(EntityTypeBuilder<SignosVitales> builder)
    {
        builder.ToTable("Signos_Vitales");
        builder.HasKey(s => s.SignosVitalesID);
        builder.Property(s => s.SignosVitalesID).ValueGeneratedOnAdd();
        builder.Property(s => s.AtencionID).IsRequired();
        builder.Property(s => s.FechaMedicion).IsRequired();
        builder.Property(s => s.Temperatura).HasPrecision(18, 2);
        builder.Property(s => s.FrecuenciaCardiaca);
        builder.Property(s => s.FrecuenciaRespiratoria);
        builder.Property(s => s.PresionSistolica);
        builder.Property(s => s.PresionDiastolica);
        builder.Property(s => s.SaturacionOxigeno).HasPrecision(18, 2);
        builder.Property(s => s.PesoKg).HasPrecision(18, 2);
        builder.Property(s => s.TallaCm).HasPrecision(18, 2);
        builder.Property(s => s.IMC).HasPrecision(18, 2);
        builder.Property(s => s.Observaciones).HasMaxLength(500);
        builder.Property(s => s.CreatedAt).IsRequired();
        builder.HasOne(s => s.Atencion)
            .WithMany(a => a.SignosVitales)
            .HasForeignKey(s => s.AtencionID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
