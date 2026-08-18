using Domain.Billing.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Billing.Configurations;

public class ServicioConfiguration : IEntityTypeConfiguration<Servicio>
{
    public void Configure(EntityTypeBuilder<Servicio> builder)
    {
        builder.ToTable("Servicios");

        builder.HasKey(s => s.ServicioID);

        builder.Property(s => s.ServicioID)
            .ValueGeneratedOnAdd();

        builder.Property(s => s.Codigo)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(s => s.Nombre)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(s => s.TipoServicio)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(s => s.Descripcion)
            .HasMaxLength(1000);

        builder.Property(s => s.Activo)
            .IsRequired();

        builder.HasIndex(s => s.Codigo)
            .IsUnique();
    }
}
