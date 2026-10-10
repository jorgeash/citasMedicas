using Business.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Business.Persistence.Configurations;

public class CustomerContactConfiguration : IEntityTypeConfiguration<CustomerContact>
{
    public void Configure(EntityTypeBuilder<CustomerContact> builder)
    {
        builder.ToTable("Business_CustomerContact");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id).ValueGeneratedOnAdd();

        builder.Property(c => c.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Email)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Phone1)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(c => c.Phone2)
            .HasMaxLength(30);

        builder.HasOne(c => c.Customer)
            .WithMany()
            .HasForeignKey(c => c.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Dato de prueba: contacto asociado al cliente CLI-0001.
        builder.HasData(
            new CustomerContact
            {
                Id = 1,
                FirstName = "Ana",
                LastName = "Torres",
                Email = "a.torres@distribuidoraandina.example",
                Phone1 = "+593 2 2345678",
                Phone2 = "+593 99 1234567",
                CustomerId = 1,
                IsActive = true,
                CreatedAt = SeedDefaults.CreatedAt
            }
        );
    }
}