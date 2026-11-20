using Clinical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinical.Persistence.Configurations;

public class AtencionProcedimientoConfiguration : IEntityTypeConfiguration<AtencionProcedimiento>
{
    public void Configure(EntityTypeBuilder<AtencionProcedimiento> builder)
    {
        builder.ToTable("Atención_Procedimiento");
        builder.HasKey(a => a.AtencionProcedimientoID);
        builder.Property(a => a.AtencionProcedimientoID).ValueGeneratedOnAdd();
        builder.Property(a => a.AtencionID).IsRequired();
        builder.Property(a => a.ProcedimientoID).IsRequired();
        builder.Property(a => a.FechaRealizacion).IsRequired();
        builder.Property(a => a.Cantidad).IsRequired();
        builder.Property(a => a.Observaciones).HasMaxLength(500);
        builder.Property(a => a.CreatedAt).IsRequired();
        builder.HasOne(a => a.Atencion)
            .WithMany(t => t.Procedimientos)
            .HasForeignKey(a => a.AtencionID)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(a => a.Procedimiento)
            .WithMany(p => p.AtencionProcedimientos)
            .HasForeignKey(a => a.ProcedimientoID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
