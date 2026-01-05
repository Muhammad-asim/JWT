# API Versioning Guide

This project now supports API versioning with separate configurations for each version.

**Note:** This project uses the modern `Asp.Versioning.Mvc` package (v8.1.0) instead of the deprecated `Microsoft.AspNetCore.Mvc.Versioning` package.

## Structure

### Version-Specific Configuration
- **`Configurations/VersionConfigurations.cs`**: Contains version-specific configuration classes
  - `VersionConfigurations`: Base API versioning configuration
  - `Version1Configurations`: Services and middleware for API v1.0
  - `Version2Configurations`: Services and middleware for API v2.0

### Controllers Organization
- **V1 Controllers** (in `Controllers/` folder):
  - `AuthController` - `/api/v1/auth`
  - `UserController` - `/api/v1/user`
  - `AdminController` - `/api/v1/admin`
  - `SecureController` - `/api/v1/secure`
  - `WeatherForecastController` - `/api/v1/weather`

- **V2 Controllers** (in `Controllers/V2/` folder):
  - `AuthControllerV2` - `/api/v2/auth` (Enhanced with DTOs and better responses)
  - `UserControllerV2` - `/api/v2/user` (Enhanced with additional endpoints)

## Program.cs Structure

The `Program.cs` file is organized into clear sections:

1. **Base Configuration**: Common services for all versions (Database, Identity, JWT, etc.)
2. **API Versioning Configuration**: Sets up versioning with multiple version readers
3. **Version-Specific Service Configurations**: Calls `AddVersion1Services()` and `AddVersion2Services()`
4. **Swagger Configuration**: Configured to show all API versions
5. **Version-Specific Middleware**: Calls `UseVersion1Middleware()` and `UseVersion2Middleware()`

## API Versioning Methods

The API supports versioning through multiple methods:

1. **URL Segment** (Recommended): `/api/v1/auth/login` or `/api/v2/auth/login`
2. **Query String**: `/api/auth/login?api-version=1.0`
3. **Header**: `X-Version: 2.0`

## Default Version

- Default version: **1.0**
- If no version is specified, requests default to v1.0

## Adding a New Version

To add a new version (e.g., v3.0):

1. **Create version-specific configuration** in `Configurations/VersionConfigurations.cs`:
   ```csharp
   public static class Version3Configurations
   {
       public static IServiceCollection AddVersion3Services(this IServiceCollection services)
       {
           // Add v3.0 specific services
           return services;
       }
       
       public static WebApplication UseVersion3Middleware(this WebApplication app)
       {
           // Add v3.0 specific middleware
           return app;
       }
   }
   ```

2. **Add to Program.cs**:
   ```csharp
   builder.Services.AddVersion3Services();
   // ...
   app.UseVersion3Middleware();
   ```

3. **Create V3 controllers** in `Controllers/V3/` folder:
   ```csharp
   [ApiController]
   [ApiVersion("3.0")]
   [Route("api/v{version:apiVersion}/auth")]
   public class AuthControllerV3 : ControllerBase
   {
       // V3 implementation
   }
   ```

4. **Add Swagger document** in `Program.cs`:
   ```csharp
   options.SwaggerDoc("v3", new OpenApiInfo { ... });
   ```

## Swagger UI

When running in Development mode, Swagger UI is available at the root URL (`/`) and displays all API versions. You can switch between versions using the dropdown at the top of the Swagger UI.

## Example API Calls

### Version 1.0
```http
POST /api/v1/auth/login
Content-Type: application/x-www-form-urlencoded

email=user@example.com&password=password123
```

### Version 2.0
```http
POST /api/v2/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "password123"
}
```

### Using Query String
```http
POST /api/auth/login?api-version=2.0
```

### Using Header
```http
POST /api/auth/login
X-Version: 2.0
```

## Benefits of This Structure

1. **Separation of Concerns**: Each version has its own configuration and controllers
2. **Easy Maintenance**: Version-specific code is clearly organized
3. **Backward Compatibility**: Old versions remain functional while new versions are added
4. **Clear Migration Path**: Easy to see what changed between versions
5. **Flexible**: Supports multiple versioning strategies (URL, query string, header)

