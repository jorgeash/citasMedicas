using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Scheduling.Domain.Entities;

namespace Scheduling.Persistence.Configurations;

public class AseguradoraConfiguration : IEntityTypeConfiguration<Aseguradora>
{
    public void Configure(EntityTypeBuilder<Aseguradora> builder)
    {
        builder.ToTable("Aseguradoras");

        builder.HasKey(a => a.AseguradoraID);

        builder.Property(a => a.AseguradoraID)
            .HasColumnName("AseguradoraID")
            .ValueGeneratedOnAdd();

        builder.Property(a => a.Codigo).HasColumnName("Codigo").HasMaxLength(30).IsRequired();
        builder.Property(a => a.Nombre).HasColumnName("Nombre").HasMaxLength(200).IsRequired();
        builder.Property(a => a.NumeroFiscal).HasColumnName("NumeroFiscal").HasMaxLength(100);
        builder.Property(a => a.Telefono).HasColumnName("Telefono").HasMaxLength(30);
        builder.Property(a => a.Email).HasColumnName("Email").HasMaxLength(150);
        builder.Property(a => a.Direccion).HasColumnName("Direccion").HasMaxLength(600);
        builder.Property(a => a.Activo).HasColumnName("Activo").IsRequired();
    }
}
