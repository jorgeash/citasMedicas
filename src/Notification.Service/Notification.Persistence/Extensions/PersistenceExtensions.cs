using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notification.Core.Interfaces.Repositories;
using Notification.Persistence.Context;
using Notification.Persistence.Repositories;

namespace Notification.Persistence.Extensions;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<NotificationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("ClinicaDB")));

        services.AddScoped<DbContext>(sp => sp.GetRequiredService<NotificationDbContext>());

        services.AddScoped<INotificacionRepository, NotificacionRepository>();

        return services;
    }
}
