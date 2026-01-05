using Microsoft.AspNetCore.Builder;

namespace JwtAuthDemo.Configurations
{
    /// <summary>
    /// Middleware pipeline configuration extensions
    /// </summary>
    public static class MiddlewareConfiguration
    {
        /// <summary>
        /// Configures the middleware pipeline
        /// </summary>
        public static WebApplication UseMiddlewareConfiguration(this WebApplication app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            return app;
        }
    }
}

