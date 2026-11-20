using Clinical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinical.Persistence.Configurations;

public class CamaConfiguration : IEntityTypeConfiguration<Cama>
{
    public void Configure(EntityTypeBuilder<Cama> builder)
    {
        builder.ToTable("Camas");
        builder.HasKey(c => c.CamaID);
        builder.Property(c => c.CamaID).ValueGeneratedOnAdd();
        builder.Property(c => c.HabitacionID).IsRequired();
        builder.Property(c => c.Codigo).HasMaxLength(30).IsRequired();
        builder.HasIndex(c => c.Codigo).IsUnique();
        builder.Property(c => c.Estado).HasMaxLength(30).IsRequired();
        builder.Property(c => c.Activa).IsRequired();
        builder.Property(c => c.CreatedAt).IsRequired();
        builder.HasMany(c => c.Hospitalizaciones)
            .WithOne(h => h.Cama)
            .HasForeignKey(h => h.CamaID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
