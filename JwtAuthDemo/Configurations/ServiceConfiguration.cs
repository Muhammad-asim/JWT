using JwtAuthDemo.Services;
using Microsoft.Extensions.DependencyInjection;

namespace JwtAuthDemo.Configurations
{
    /// <summary>
    /// Application services configuration extensions
    /// </summary>
    public static class ServiceConfiguration
    {
        /// <summary>
        /// Configures application services
        /// </summary>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            return services;
        }
    }
}

