using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Scheduling.Domain.Entities;

namespace Scheduling.Persistence.Configurations;

public class EstadoCitaConfiguration : IEntityTypeConfiguration<EstadoCita>
{
    public void Configure(EntityTypeBuilder<EstadoCita> builder)
    {
        builder.ToTable("Estados_Cita");

        builder.HasKey(e => e.EstadoCitaID);

        builder.Property(e => e.EstadoCitaID)
            .HasColumnName("EstadoCitaID")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Codigo).HasColumnName("Codigo").HasMaxLength(30).IsRequired();
        builder.Property(e => e.Nombre).HasColumnName("Nombre").HasMaxLength(100).IsRequired();
    }
}
