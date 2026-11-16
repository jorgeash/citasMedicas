using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Scheduling.Domain.Entities;

namespace Scheduling.Persistence.Configurations;

public class CitaConfiguration : IEntityTypeConfiguration<Cita>
{
    public void Configure(EntityTypeBuilder<Cita> builder)
    {
        builder.ToTable("Citas");

        builder.HasKey(c => c.CitaID);

        builder.Property(c => c.CitaID)
            .HasColumnName("CitaID")
            .ValueGeneratedOnAdd();

        builder.Property(c => c.PacienteID).HasColumnName("PacienteID").IsRequired();
        builder.Property(c => c.MedicoID).HasColumnName("MedicoID").IsRequired();
        builder.Property(c => c.EspecialidadID).HasColumnName("EspecialidadID");
        builder.Property(c => c.ConsultorioID).HasColumnName("ConsultorioID");
        builder.Property(c => c.EstadoCitaID).HasColumnName("EstadoCitaID").IsRequired();
        builder.Property(c => c.FechaHoraInicio).HasColumnName("FechaHoraInicio").HasColumnType("datetime2").IsRequired();
        builder.Property(c => c.FechaHoraFin).HasColumnName("FechaHoraFin").HasColumnType("datetime2");
        builder.Property(c => c.Motivo).HasColumnName("Motivo").HasMaxLength(2000);
        builder.Property(c => c.Observaciones).HasColumnName("Observaciones").HasMaxLength(2000);
        builder.Property(c => c.UsuarioCreacionID).HasColumnName("UsuarioCreacionID");
        builder.Property(c => c.FechaCreacion).HasColumnName("FechaCreacion").HasColumnType("datetime2").IsRequired();

        builder.HasOne(c => c.Paciente)
            .WithMany()
            .HasForeignKey(c => c.PacienteID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Medico)
            .WithMany()
            .HasForeignKey(c => c.MedicoID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.EstadoCita)
            .WithMany()
            .HasForeignKey(c => c.EstadoCitaID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
