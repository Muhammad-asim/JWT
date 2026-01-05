using Asp.Versioning.ApiExplorer;
using JwtAuthDemo.Configurations;
using JwtAuthDemo.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// BASE CONFIGURATION (Common for all versions)
// ============================================

// Database Configuration
builder.Services.AddDbContext<JwtAuthDemo.Data.AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity Configuration
builder.Services.AddIdentity<Microsoft.AspNetCore.Identity.IdentityUser, Microsoft.AspNetCore.Identity.IdentityRole>()
    .AddEntityFrameworkStores<JwtAuthDemo.Data.AppDbContext>()
    .AddDefaultTokenProviders();

// JWT Authentication Configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["access_token"];
                return System.Threading.Tasks.Task.CompletedTask;
            }
        };
    });

// Authorization
builder.Services.AddAuthorization();

// Stop Identity Redirect
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401;
        return System.Threading.Tasks.Task.CompletedTask;
    };
});

// Controllers
builder.Services.AddControllers();

// ============================================
// API VERSIONING CONFIGURATION
// ============================================
builder.Services.AddApiVersioningConfiguration();

// ============================================
// VERSION-SPECIFIC SERVICE CONFIGURATIONS
// ============================================
builder.Services.AddVersion1Services(); // Version 1.0 services
builder.Services.AddVersion2Services(); // Version 2.0 services

// ============================================
// SWAGGER CONFIGURATION (with versioning)
// ============================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Configure Swagger documents for each API version
    // This will be populated by the version description provider
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "JWT Auth Demo API",
        Version = "1.0",
        Description = "JWT Authentication Demo API - Version 1.0",
        Contact = new OpenApiContact
        {
            Name = "API Support",
            Email = "support@example.com"
        }
    });
    
    options.SwaggerDoc("v2", new OpenApiInfo
    {
        Title = "JWT Auth Demo API",
        Version = "2.0",
        Description = "JWT Authentication Demo API - Version 2.0",
        Contact = new OpenApiContact
        {
            Name = "API Support",
            Email = "support@example.com"
        }
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

// ============================================
// APPLICATION SERVICES
// ============================================
builder.Services.AddScoped<JwtAuthDemo.Services.IJwtTokenService, JwtAuthDemo.Services.JwtTokenService>();

// ============================================
// BUILD APPLICATION
// ============================================
var app = builder.Build();

// ============================================
// VERSION-SPECIFIC MIDDLEWARE CONFIGURATIONS
// ============================================
app.UseVersion1Middleware(); // Version 1.0 middleware
app.UseVersion2Middleware(); // Version 2.0 middleware

// ============================================
// PIPELINE CONFIGURATION
// ============================================
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ============================================
// SEED DATA (Optional - uncomment if needed)
// ============================================
//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    await SeedData.SeedRoles(services);
//}

app.Run();
