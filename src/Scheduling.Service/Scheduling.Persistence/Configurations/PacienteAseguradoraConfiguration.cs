using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Scheduling.Domain.Entities;

namespace Scheduling.Persistence.Configurations;

public class PacienteAseguradoraConfiguration : IEntityTypeConfiguration<PacienteAseguradora>
{
    public void Configure(EntityTypeBuilder<PacienteAseguradora> builder)
    {
        builder.ToTable("Paciente_Aseguradora");

        builder.HasKey(s => s.PacienteAseguradoraID);

        builder.Property(s => s.PacienteAseguradoraID)
            .HasColumnName("PacienteAseguradoraID")
            .ValueGeneratedOnAdd();

        builder.Property(s => s.PacienteID).HasColumnName("PacienteID").IsRequired();
        builder.Property(s => s.AseguradoraID).HasColumnName("AseguradoraID").IsRequired();
        builder.Property(s => s.NumeroPoliza).HasColumnName("NumeroPoliza").HasMaxLength(100).IsRequired();
        builder.Property(s => s.NumeroAfiliacion).HasColumnName("NumeroAfiliacion").HasMaxLength(100);
        builder.Property(s => s.TitularNombre).HasColumnName("TitularNombre").HasMaxLength(200);
        builder.Property(s => s.ParentescoTitular).HasColumnName("ParentescoTitular").HasMaxLength(100);
        builder.Property(s => s.FechaInicio).HasColumnName("FechaInicio").HasColumnType("date");
        builder.Property(s => s.FechaFin).HasColumnName("FechaFin").HasColumnType("date");
        builder.Property(s => s.CoberturaPorcentaje).HasColumnName("CoberturaPorcentaje").HasPrecision(5, 2);
        builder.Property(s => s.Activo).HasColumnName("Activo").IsRequired();

        builder.HasOne(s => s.Paciente)
            .WithMany(p => p.Seguros)
            .HasForeignKey(s => s.PacienteID);

        builder.HasOne(s => s.Aseguradora)
            .WithMany()
            .HasForeignKey(s => s.AseguradoraID);
    }
}
