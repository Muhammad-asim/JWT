using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace JwtAuthDemo.Configurations
{
    /// <summary>
    /// Base configuration for all API versions
    /// </summary>
    public static class VersionConfigurations
    {
        /// <summary>
        /// Configures API versioning services
        /// </summary>
        public static IServiceCollection AddApiVersioningConfiguration(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new QueryStringApiVersionReader("api-version"),
                    new HeaderApiVersionReader("X-Version"),
                    new UrlSegmentApiVersionReader()
                );
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            return services;
        }

        /// <summary>
        /// Configures Swagger with versioning support
        /// </summary>
        public static IServiceCollection AddSwaggerVersioningConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                // Use a factory to resolve IApiVersionDescriptionProvider
                options.SwaggerGeneratorOptions.SwaggerDocs.Clear();
                
                // We'll configure the docs in Program.cs after building the app
                // For now, add the security definitions
                
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

                // Include XML comments if available
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
            });

            return services;
        }

        /// <summary>
        /// Configures Swagger UI with version-specific endpoints
        /// This should be called after the app is built
        /// </summary>
        public static void ConfigureSwaggerWithVersions(this WebApplication app)
        {
            // This method is called from Program.cs after app is built
            // The Swagger UI configuration is handled in Program.cs
        }
    }

    /// <summary>
    /// Version 1.0 specific configurations
    /// </summary>
    public static class Version1Configurations
    {
        /// <summary>
        /// Configures services specific to API Version 1.0
        /// </summary>
        public static IServiceCollection AddVersion1Services(this IServiceCollection services)
        {
            // Add version 1.0 specific services here
            // For example: services.AddScoped<IV1Service, V1Service>();
            
            return services;
        }

        /// <summary>
        /// Configures middleware specific to API Version 1.0
        /// </summary>
        public static WebApplication UseVersion1Middleware(this WebApplication app)
        {
            // Add version 1.0 specific middleware here
            // For example: app.UseMiddleware<V1Middleware>();
            
            return app;
        }
    }

    /// <summary>
    /// Version 2.0 specific configurations
    /// </summary>
    public static class Version2Configurations
    {
        /// <summary>
        /// Configures services specific to API Version 2.0
        /// </summary>
        public static IServiceCollection AddVersion2Services(this IServiceCollection services)
        {
            // Add version 2.0 specific services here
            // For example: services.AddScoped<IV2Service, V2Service>();
            
            return services;
        }

        /// <summary>
        /// Configures middleware specific to API Version 2.0
        /// </summary>
        public static WebApplication UseVersion2Middleware(this WebApplication app)
        {
            // Add version 2.0 specific middleware here
            // For example: app.UseMiddleware<V2Middleware>();
            
            return app;
        }
    }
}

