using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Scheduling.Domain.Entities;

namespace Scheduling.Persistence.Configurations;

public class MedicoConfiguration : IEntityTypeConfiguration<Medico>
{
    public void Configure(EntityTypeBuilder<Medico> builder)
    {
        builder.ToTable("Medicos");

        builder.HasKey(m => m.MedicoID);

        builder.Property(m => m.MedicoID)
            .HasColumnName("MedicoID")
            .ValueGeneratedOnAdd();

        builder.Property(m => m.UsuarioID).HasColumnName("UsuarioID");
        builder.Property(m => m.CodigoMedico).HasColumnName("CodigoMedico").HasMaxLength(30).IsRequired();
        builder.Property(m => m.Nombres).HasColumnName("Nombres").HasMaxLength(200).IsRequired();
        builder.Property(m => m.Apellidos).HasColumnName("Apellidos").HasMaxLength(200).IsRequired();
        builder.Property(m => m.NumeroLicencia).HasColumnName("NumeroLicencia").HasMaxLength(50);
        builder.Property(m => m.Telefono).HasColumnName("Telefono").HasMaxLength(30);
        builder.Property(m => m.Email).HasColumnName("Email").HasMaxLength(150);
        builder.Property(m => m.Activo).HasColumnName("Activo").IsRequired();
    }
}
