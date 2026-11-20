using Clinical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinical.Persistence.Configurations;

public class MedicamentoConfiguration : IEntityTypeConfiguration<Medicamento>
{
    public void Configure(EntityTypeBuilder<Medicamento> builder)
    {
        builder.ToTable("Medicamentos");
        builder.HasKey(m => m.MedicamentoID);
        builder.Property(m => m.MedicamentoID).ValueGeneratedOnAdd();
        builder.Property(m => m.Codigo).HasMaxLength(30).IsRequired();
        builder.HasIndex(m => m.Codigo).IsUnique();
        builder.Property(m => m.Nombre).HasMaxLength(200).IsRequired();
        builder.Property(m => m.Presentacion).HasMaxLength(100);
        builder.Property(m => m.Concentracion).HasMaxLength(100);
        builder.Property(m => m.ViaAdministracion).HasMaxLength(50);
        builder.Property(m => m.Activo).IsRequired();
        builder.Property(m => m.CreatedAt).IsRequired();
    }
}
