using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace JwtAuthDemo.Configurations
{
    /// <summary>
    /// Identity configuration extensions
    /// </summary>
    public static class IdentityConfiguration
    {
        /// <summary>
        /// Configures ASP.NET Core Identity
        /// </summary>
        public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services)
        {
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<Data.AppDbContext>()
                .AddDefaultTokenProviders();

            // Stop Identity Redirect
            services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return System.Threading.Tasks.Task.CompletedTask;
                };
            });

            return services;
        }
    }
}

