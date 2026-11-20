using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notification.Domain.Entities;

namespace Notification.Persistence.Configurations;

public class NotificacionConfiguration : IEntityTypeConfiguration<Notificacion>
{
    public void Configure(EntityTypeBuilder<Notificacion> builder)
    {
        builder.ToTable("Notificaciones");

        builder.HasKey(n => n.NotificacionID);

        builder.Property(n => n.NotificacionID)
            .ValueGeneratedOnAdd();

        builder.Property(n => n.EventoID)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(n => n.EventoID)
            .IsUnique();

        builder.Property(n => n.UsuarioID)
            .IsRequired();

        builder.HasIndex(n => n.UsuarioID);

        builder.Property(n => n.Destinatario)
            .HasMaxLength(256);

        builder.Property(n => n.Titulo)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(n => n.Mensaje)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(n => n.Tipo)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(n => n.Canal)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(n => n.Prioridad)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(n => n.Estado)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(n => n.OrigenServicio)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(n => n.Referencia)
            .HasMaxLength(100);

        builder.Property(n => n.FechaCreacion)
            .IsRequired();

        builder.Property(n => n.MotivoCancelacion)
            .HasMaxLength(300);

        builder.Property(n => n.Intentos)
            .IsRequired();

        builder.Property(n => n.UltimoError)
            .HasMaxLength(500);

        builder.Property(n => n.Activo)
            .IsRequired();

        builder.Property(n => n.CreatedAt)
            .IsRequired();
    }
}
