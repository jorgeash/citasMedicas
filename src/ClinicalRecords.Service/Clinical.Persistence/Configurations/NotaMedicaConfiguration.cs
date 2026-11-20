using Clinical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinical.Persistence.Configurations;

public class NotaMedicaConfiguration : IEntityTypeConfiguration<NotaMedica>
{
    public void Configure(EntityTypeBuilder<NotaMedica> builder)
    {
        builder.ToTable("Notas_Médicas");
        builder.HasKey(n => n.NotaMedicaID);
        builder.Property(n => n.NotaMedicaID).ValueGeneratedOnAdd();
        builder.Property(n => n.AtencionID).IsRequired();
        builder.Property(n => n.MedicoID).IsRequired();
        builder.Property(n => n.FechaNota).IsRequired();
        builder.Property(n => n.Tipo).HasMaxLength(50).IsRequired();
        builder.Property(n => n.Contenido).HasMaxLength(4000).IsRequired();
        builder.Property(n => n.CreatedAt).IsRequired();
        builder.HasOne(n => n.Atencion)
            .WithMany(a => a.NotasMedicas)
            .HasForeignKey(n => n.AtencionID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
