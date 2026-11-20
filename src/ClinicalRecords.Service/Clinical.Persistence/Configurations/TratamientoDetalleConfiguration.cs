using Clinical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinical.Persistence.Configurations;

public class TratamientoDetalleConfiguration : IEntityTypeConfiguration<TratamientoDetalle>
{
    public void Configure(EntityTypeBuilder<TratamientoDetalle> builder)
    {
        builder.ToTable("Tratamiento_Detalle");
        builder.HasKey(d => d.TratamientoDetalleID);
        builder.Property(d => d.TratamientoDetalleID).ValueGeneratedOnAdd();
        builder.Property(d => d.TratamientoID).IsRequired();
        builder.Property(d => d.Tipo).HasMaxLength(50).IsRequired();
        builder.Property(d => d.Descripcion).HasMaxLength(2000).IsRequired();
        builder.Property(d => d.Frecuencia).HasMaxLength(100);
        builder.Property(d => d.Observaciones).HasMaxLength(500);
        builder.Property(d => d.CreatedAt).IsRequired();
    }
}
