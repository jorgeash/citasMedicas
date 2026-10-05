using NugetPackage_Rest.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NugetPackage_Rest.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>Enlaza la sección "RestSettings" del appsettings.json con
        /// IOptions&lt;RequestSettings&gt;, que RestBuilder recibe por constructor.</summary>
        public static IServiceCollection AddRequestLogging(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RequestSettings>(configuration.GetSection("RestSettings"));
            return services;
        }
    }
}