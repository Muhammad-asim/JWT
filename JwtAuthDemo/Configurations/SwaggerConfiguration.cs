using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace JwtAuthDemo.Configurations
{
    /// <summary>
    /// Swagger/OpenAPI configuration extensions
    /// </summary>
    public static class SwaggerConfiguration
    {
        /// <summary>
        /// Configures Swagger/OpenAPI documentation
        /// </summary>
        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                // Configure Swagger documents for each API version
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "JWT Auth Demo API",
                    Version = "1.0",
                    Description = "JWT Authentication Demo API - Version 1.0"
                });

                options.SwaggerDoc("v2", new OpenApiInfo
                {
                    Title = "JWT Auth Demo API",
                    Version = "2.0",
                    Description = "JWT Authentication Demo API - Version 2.0"
                });

                // Add JWT Bearer authentication to Swagger
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            return services;
        }

        /// <summary>
        /// Configures Swagger UI middleware
        /// </summary>
        public static WebApplication UseSwaggerConfiguration(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
                    foreach (var description in provider.ApiVersionDescriptions.OrderByDescending(d => d.ApiVersion))
                    {
                        options.SwaggerEndpoint(
                            $"/swagger/{description.GroupName}/swagger.json",
                            $"JWT Auth Demo API {description.GroupName.ToUpperInvariant()}"
                        );
                    }
                    options.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
                });
            }

            return app;
        }
    }
}

