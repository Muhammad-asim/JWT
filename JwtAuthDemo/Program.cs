using JwtAuthDemo.Configurations;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// BASE CONFIGURATION (Common for all versions)
// ============================================
builder.Services.AddDatabaseConfiguration(builder.Configuration);
builder.Services.AddIdentityConfiguration();
builder.Services.AddJwtAuthenticationConfiguration(builder.Configuration);
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
builder.Services.AddSwaggerConfiguration();

// ============================================
// APPLICATION SERVICES
// ============================================
builder.Services.AddApplicationServices();

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
app.UseSwaggerConfiguration();
app.UseMiddlewareConfiguration();

// ============================================
// SEED DATA (Optional - uncomment if needed)
// ============================================
//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    await SeedData.SeedRoles(services);
//}

app.Run();
