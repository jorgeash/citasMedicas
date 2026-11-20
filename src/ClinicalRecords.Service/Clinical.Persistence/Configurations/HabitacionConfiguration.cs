using Clinical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinical.Persistence.Configurations;

public class HabitacionConfiguration : IEntityTypeConfiguration<Habitacion>
{
    public void Configure(EntityTypeBuilder<Habitacion> builder)
    {
        builder.ToTable("Habitaciones");
        builder.HasKey(h => h.HabitacionID);
        builder.Property(h => h.HabitacionID).ValueGeneratedOnAdd();
        builder.Property(h => h.Codigo).HasMaxLength(30).IsRequired();
        builder.HasIndex(h => h.Codigo).IsUnique();
        builder.Property(h => h.Numero).HasMaxLength(20).IsRequired();
        builder.Property(h => h.Tipo).HasMaxLength(50).IsRequired();
        builder.Property(h => h.Piso).HasMaxLength(20);
        builder.Property(h => h.AreaID);
        builder.Property(h => h.Activa).IsRequired();
        builder.Property(h => h.CreatedAt).IsRequired();
        builder.HasMany(h => h.Camas)
            .WithOne(c => c.Habitacion)
            .HasForeignKey(c => c.HabitacionID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
