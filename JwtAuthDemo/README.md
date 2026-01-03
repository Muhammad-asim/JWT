# JWT Authentication Demo

A production-ready ASP.NET Core 8.0 Web API application demonstrating industry-standard JWT (JSON Web Token) authentication with refresh token support, role-based authorization, and ASP.NET Core Identity integration.

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [API Endpoints](#api-endpoints)
- [Project Structure](#project-structure)
- [Security Features](#security-features)
- [Testing](#testing)
- [Deployment](#deployment)
- [Additional Documentation](#additional-documentation)

## 🎯 Overview

This project demonstrates a secure authentication and authorization system built with:
- **ASP.NET Core 8.0** - Modern web framework
- **JWT Bearer Authentication** - Stateless token-based authentication
- **ASP.NET Core Identity** - User and role management
- **Entity Framework Core** - Database ORM
- **SQL Server** - Database backend

The application follows industry best practices for security, architecture, and code organization.

## ✨ Features

### Authentication & Authorization
- ✅ User registration with email validation
- ✅ Secure login with password hashing (via ASP.NET Identity)
- ✅ JWT access token generation
- ✅ Refresh token mechanism (partial implementation)
- ✅ Role-based authorization (Admin, User)
- ✅ Cookie-based token storage (HttpOnly, Secure, SameSite)
- ✅ Token expiration and validation

### Security
- ✅ Password hashing using ASP.NET Identity's PBKDF2
- ✅ HttpOnly cookies to prevent XSS attacks
- ✅ Secure flag for HTTPS-only cookies
- ✅ SameSite protection against CSRF
- ✅ Token validation (issuer, audience, lifetime, signing key)
- ✅ Role-based access control (RBAC)

### API Features
- ✅ RESTful API design
- ✅ Swagger/OpenAPI documentation
- ✅ Protected endpoints with authorization
- ✅ User profile management
- ✅ Admin dashboard access

## 🛠 Technology Stack

| Component | Technology | Version |
|-----------|-----------|---------|
| Framework | ASP.NET Core | 8.0 |
| Language | C# | 12.0 |
| Authentication | JWT Bearer | 8.0.22 |
| Identity | ASP.NET Core Identity | 8.0.22 |
| ORM | Entity Framework Core | 8.0.22 |
| Database | SQL Server | - |
| API Documentation | Swashbuckle (Swagger) | 6.6.2 |
| JWT Library | System.IdentityModel.Tokens.Jwt | 8.15.0 |

## 📦 Prerequisites

Before running this application, ensure you have:

- **.NET 8.0 SDK** or later ([Download](https://dotnet.microsoft.com/download))
- **SQL Server** (Express, LocalDB, or full instance)
- **Visual Studio 2022** or **Visual Studio Code** (recommended)
- **Git** (for cloning the repository)

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd JwtAuthDemo
```

### 2. Configure Database Connection

Update `appsettings.Development.json` or `appsettings.json` with your SQL Server connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=JwtIdentityDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### 3. Configure JWT Settings

Update JWT configuration in `appsettings.Development.json`:

```json
{
  "Jwt": {
    "Key": "Your-Super-Secret-Key-At-Least-32-Characters-Long",
    "Issuer": "JwtAuthDemo",
    "Audience": "JwtAuthDemoUsers",
    "ExpireMinutes": 15,
    "RefreshTokenDays": 7
  }
}
```

**⚠️ Security Warning**: In production, store the JWT key in:
- Azure Key Vault
- Environment variables
- Secret Manager (development)
- Never commit secrets to source control

### 4. Run Database Migrations

```bash
dotnet ef database update
```

### 5. Seed Initial Roles (Optional)

Uncomment the seed data code in `Program.cs`:

```csharp
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.SeedRoles(services);
}
```

### 6. Run the Application

```bash
dotnet run
```

Or use Visual Studio:
- Press `F5` to run with debugging
- Press `Ctrl+F5` to run without debugging

The application will be available at:
- HTTP: `http://localhost:5225`
- HTTPS: `https://localhost:7093`
- Swagger UI: `https://localhost:7093/swagger`

## ⚙️ Configuration

### JWT Settings

| Setting | Description | Default |
|---------|-------------|---------|
| `Jwt:Key` | Secret key for signing tokens (min 32 chars) | Required |
| `Jwt:Issuer` | Token issuer identifier | JwtAuthDemo |
| `Jwt:Audience` | Token audience identifier | JwtAuthDemoUsers |
| `Jwt:ExpireMinutes` | Access token expiration (minutes) | 15 |
| `Jwt:RefreshTokenDays` | Refresh token expiration (days) | 7 |

### Environment Variables

For production, use environment variables:

```bash
# Windows PowerShell
$env:Jwt__Key="Your-Production-Key-Here"
$env:ConnectionStrings__DefaultConnection="Server=prod-server;Database=JwtDb;..."

# Linux/Mac
export Jwt__Key="Your-Production-Key-Here"
export ConnectionStrings__DefaultConnection="Server=prod-server;Database=JwtDb;..."
```

## 📡 API Endpoints

### Authentication Endpoints

#### Register User
```http
POST /api/auth/register
Content-Type: application/x-www-form-urlencoded

email=user@example.com&password=SecurePass123!
```

**Response:**
```json
{
  "message": "User registered successfully"
}
```

#### Login
```http
POST /api/auth/login
Content-Type: application/x-www-form-urlencoded

email=user@example.com&password=SecurePass123!
```

**Response:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "base64-encoded-refresh-token"
}
```

**Note:** Access token is also set as an HttpOnly cookie.

#### Refresh Token
```http
POST /api/auth/refresh
Content-Type: application/x-www-form-urlencoded

refreshToken=your-refresh-token-here
```

**Response:**
```json
{
  "accessToken": "new-access-token"
}
```

### Protected Endpoints

#### Get Secure Data
```http
GET /api/secure
Authorization: Bearer {accessToken}
```

#### Get User Profile
```http
GET /api/User/profile
Authorization: Bearer {accessToken}
```
**Requires:** `User` role

#### Get Admin Dashboard
```http
GET /api/admin/dashboard
Authorization: Bearer {accessToken}
```
**Requires:** `Admin` role

For detailed API documentation, see [API_DOCUMENTATION.md](./API_DOCUMENTATION.md).

## 📁 Project Structure

```
JwtAuthDemo/
├── Controllers/           # API Controllers
│   ├── AuthController.cs      # Authentication endpoints
│   ├── UserController.cs      # User-specific endpoints
│   ├── AdminController.cs     # Admin endpoints
│   └── SecureController.cs    # General protected endpoints
├── Services/              # Business Logic Services
│   ├── IJwtTokenService.cs    # JWT service interface
│   └── JwtTokenService.cs     # JWT token generation logic
├── Data/                  # Data Access Layer
│   ├── AppDbContext.cs        # EF Core DbContext
│   └── SeedData.cs            # Database seeding
├── Model/                 # Domain Models
│   └── RefreshToken.cs        # Refresh token model
├── Migrations/            # EF Core Migrations
├── Properties/            # Project properties
│   └── launchSettings.json   # Launch configuration
├── Program.cs             # Application entry point
├── appsettings.json       # Production configuration
├── appsettings.Development.json  # Development configuration
└── JwtAuthDemo.csproj     # Project file
```

## 🔒 Security Features

### Implemented Security Measures

1. **Password Security**
   - ASP.NET Identity uses PBKDF2 with salt
   - Password complexity requirements (configurable)

2. **Token Security**
   - HttpOnly cookies prevent JavaScript access
   - Secure flag ensures HTTPS-only transmission
   - SameSite=Strict prevents CSRF attacks
   - Token expiration enforced

3. **Authorization**
   - Role-based access control (RBAC)
   - JWT claims validation
   - Endpoint-level authorization

4. **Input Validation**
   - ASP.NET Core model validation
   - SQL injection prevention (EF Core parameterized queries)

### Security Enhancements Needed

See [SECURITY.md](./SECURITY.md) for detailed security analysis and enhancement recommendations.

## 🧪 Testing

### Using Swagger UI

1. Navigate to `/swagger` when the app is running
2. Use the "Authorize" button to set your JWT token
3. Test endpoints directly from the UI

### Using HTTP Client

Example using `JwtAuthDemo.http`:

```http
### Register
POST https://localhost:7093/api/auth/register
Content-Type: application/x-www-form-urlencoded

email=test@example.com&password=Test123!

### Login
POST https://localhost:7093/api/auth/login
Content-Type: application/x-www-form-urlencoded

email=test@example.com&password=Test123!

### Get Secure Data
GET https://localhost:7093/api/secure
Authorization: Bearer {{accessToken}}
```

### Using cURL

```bash
# Register
curl -X POST "https://localhost:7093/api/auth/register" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "email=test@example.com&password=Test123!"

# Login
curl -X POST "https://localhost:7093/api/auth/login" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "email=test@example.com&password=Test123!" \
  -c cookies.txt

# Access Protected Endpoint
curl -X GET "https://localhost:7093/api/secure" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -b cookies.txt
```

## 🚢 Deployment

### Production Checklist

- [ ] Change JWT secret key to a strong, random value
- [ ] Store secrets in Azure Key Vault or environment variables
- [ ] Enable HTTPS only
- [ ] Configure CORS for your frontend domain
- [ ] Set up proper logging and monitoring
- [ ] Configure database connection pooling
- [ ] Enable rate limiting
- [ ] Set up health checks
- [ ] Configure error handling middleware
- [ ] Review and update security headers

### Azure App Service Deployment

1. Create an Azure App Service
2. Configure connection strings in Azure Portal
3. Set application settings (JWT key, etc.)
4. Deploy using Visual Studio or Azure DevOps

### Docker Deployment

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["JwtAuthDemo.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JwtAuthDemo.dll"]
```

## 📚 Additional Documentation

- [ARCHITECTURE.md](./ARCHITECTURE.md) - System architecture and design patterns
- [SECURITY.md](./SECURITY.md) - Security analysis and enhancement plan
- [API_DOCUMENTATION.md](./API_DOCUMENTATION.md) - Detailed API reference
- [FUTURE_IMPROVEMENTS.md](./FUTURE_IMPROVEMENTS.md) - Enhancement suggestions

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📝 License

This project is provided as-is for educational and demonstration purposes.

## 👤 Author

**Asim**

## 🙏 Acknowledgments

- ASP.NET Core team for the excellent framework
- Microsoft Identity team for ASP.NET Core Identity
- JWT.io for JWT specification and tools

---

**⚠️ Important**: This is a demonstration project. For production use, implement all security enhancements outlined in [SECURITY.md](./SECURITY.md).

