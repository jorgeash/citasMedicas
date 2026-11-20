using Clinical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinical.Persistence.Configurations;

public class ReferenciaMedicaConfiguration : IEntityTypeConfiguration<ReferenciaMedica>
{
    public void Configure(EntityTypeBuilder<ReferenciaMedica> builder)
    {
        builder.ToTable("Referencias_Médicas");
        builder.HasKey(r => r.ReferenciaMedicaID);
        builder.Property(r => r.ReferenciaMedicaID).ValueGeneratedOnAdd();
        builder.Property(r => r.AtencionID).IsRequired();
        builder.Property(r => r.EspecialidadID).IsRequired();
        builder.Property(r => r.MedicoDestinoID);
        builder.Property(r => r.FechaReferencia).IsRequired();
        builder.Property(r => r.Motivo).HasMaxLength(1000).IsRequired();
        builder.Property(r => r.Estado).HasMaxLength(30).IsRequired();
        builder.Property(r => r.Observaciones).HasMaxLength(500);
        builder.Property(r => r.CreatedAt).IsRequired();
        builder.HasOne(r => r.Atencion)
            .WithMany(a => a.ReferenciasMedicas)
            .HasForeignKey(r => r.AtencionID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
