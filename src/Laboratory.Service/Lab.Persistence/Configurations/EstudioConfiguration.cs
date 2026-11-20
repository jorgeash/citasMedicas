using Lab.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lab.Persistence.Configurations;

public class EstudioConfiguration : IEntityTypeConfiguration<Estudio>
{
    public void Configure(EntityTypeBuilder<Estudio> builder)
    {
        builder.ToTable("Catálogo_Estudios");

        builder.HasKey(e => e.EstudioID);

        builder.Property(e => e.EstudioID)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Codigo)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(e => e.Codigo)
            .IsUnique();

        builder.Property(e => e.Nombre)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Tipo)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(e => e.Descripcion)
            .HasMaxLength(500);

        builder.Property(e => e.RequiereAyuno)
            .IsRequired();

        builder.Property(e => e.Activo)
            .IsRequired();

        builder.Property(e => e.Precio)
            .HasPrecision(18, 2);

        builder.Property(e => e.CreatedAt)
            .IsRequired();
    }
}
