using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Scheduling.Domain.Entities;

namespace Scheduling.Persistence.Configurations;

public class PacienteConfiguration : IEntityTypeConfiguration<Paciente>
{
    public void Configure(EntityTypeBuilder<Paciente> builder)
    {
        builder.ToTable("Pacientes");

        builder.HasKey(p => p.PacienteID);

        builder.Property(p => p.PacienteID)
            .HasColumnName("PacienteID")
            .ValueGeneratedOnAdd();

        builder.Property(p => p.CodigoPaciente).HasColumnName("CodigoPaciente").HasMaxLength(50).IsRequired();
        builder.Property(p => p.TipoDocumento).HasColumnName("TipoDocumento").HasMaxLength(30).IsRequired();
        builder.Property(p => p.NumeroDocumento).HasColumnName("NumeroDocumento").HasMaxLength(50).IsRequired();
        builder.Property(p => p.Nombres).HasColumnName("Nombres").HasMaxLength(200).IsRequired();
        builder.Property(p => p.Apellidos).HasColumnName("Apellidos").HasMaxLength(200).IsRequired();
        builder.Property(p => p.FechaNacimiento).HasColumnName("FechaNacimiento").HasColumnType("date").IsRequired();
        builder.Property(p => p.Sexo).HasColumnName("Sexo").HasColumnType("char(1)").IsRequired();
        builder.Property(p => p.EstadoCivil).HasColumnName("EstadoCivil").HasMaxLength(30);
        builder.Property(p => p.Telefono).HasColumnName("Telefono").HasMaxLength(30);
        builder.Property(p => p.TelefonoSecundario).HasColumnName("TelefonoSecundario").HasMaxLength(30);
        builder.Property(p => p.Email).HasColumnName("Email").HasMaxLength(150);
        builder.Property(p => p.Direccion).HasColumnName("Direccion").HasMaxLength(600);
        builder.Property(p => p.Ciudad).HasColumnName("Ciudad").HasMaxLength(200);
        builder.Property(p => p.Pais).HasColumnName("Pais").HasMaxLength(200);
        builder.Property(p => p.Ocupacion).HasColumnName("Ocupacion").HasMaxLength(300);
        builder.Property(p => p.TipoSangre).HasColumnName("TipoSangre").HasMaxLength(10);
        builder.Property(p => p.Activo).HasColumnName("Activo").IsRequired();
        builder.Property(p => p.FechaRegistro).HasColumnName("FechaRegistro").HasColumnType("datetime2").IsRequired();

        builder.HasMany(p => p.Seguros)
            .WithOne(s => s.Paciente)
            .HasForeignKey(s => s.PacienteID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
