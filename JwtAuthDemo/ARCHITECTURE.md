# Architecture Documentation

## System Architecture Overview

This document describes the architecture, design patterns, and technical decisions for the JWT Authentication Demo application.

## Table of Contents

- [Architecture Overview](#architecture-overview)
- [System Design](#system-design)
- [Layered Architecture](#layered-architecture)
- [Design Patterns](#design-patterns)
- [Data Flow](#data-flow)
- [Technology Decisions](#technology-decisions)
- [Industry Standards Compliance](#industry-standards-compliance)

## Architecture Overview

The application follows a **layered architecture** pattern with clear separation of concerns:

```
┌─────────────────────────────────────────┐
│         Presentation Layer              │
│      (Controllers/API Endpoints)        │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│         Business Logic Layer            │
│         (Services/Interfaces)           │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│         Data Access Layer               │
│    (DbContext/Entity Framework)         │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│         Database Layer                  │
│          (SQL Server)                   │
└─────────────────────────────────────────┘
```

## System Design

### High-Level Architecture

```
┌──────────────┐
│   Client     │
│  (Browser/   │
│   Mobile)    │
└──────┬───────┘
       │ HTTPS
       │
┌──────▼──────────────────────────────────┐
│      ASP.NET Core Web API               │
│  ┌──────────────────────────────────┐  │
│  │  Authentication Middleware        │  │
│  │  (JWT Bearer)                    │  │
│  └──────────────────────────────────┘  │
│  ┌──────────────────────────────────┐  │
│  │  Authorization Middleware        │  │
│  │  (Role-based)                    │  │
│  └──────────────────────────────────┘  │
│  ┌──────────────────────────────────┐  │
│  │  Controllers                      │  │
│  │  - AuthController                │  │
│  │  - UserController                │  │
│  │  - AdminController               │  │
│  └──────────────────────────────────┘  │
│  ┌──────────────────────────────────┐  │
│  │  Services                        │  │
│  │  - JwtTokenService               │  │
│  └──────────────────────────────────┘  │
│  ┌──────────────────────────────────┐  │
│  │  ASP.NET Core Identity           │  │
│  │  - UserManager                   │  │
│  │  - SignInManager                 │  │
│  │  - RoleManager                   │  │
│  └──────────────────────────────────┘  │
└──────┬──────────────────────────────────┘
       │
┌──────▼──────────────────────────────────┐
│      Entity Framework Core              │
│      (ORM Layer)                        │
└──────┬──────────────────────────────────┘
       │
┌──────▼──────────────────────────────────┐
│      SQL Server Database                │
│  - AspNetUsers                          │
│  - AspNetRoles                          │
│  - AspNetUserRoles                      │
│  - AspNetUserClaims                     │
└─────────────────────────────────────────┘
```

## Layered Architecture

### 1. Presentation Layer (Controllers)

**Location:** `Controllers/`

**Responsibilities:**
- Handle HTTP requests and responses
- Input validation
- Authorization enforcement
- Response formatting

**Components:**
- `AuthController` - Authentication operations
- `UserController` - User-specific operations
- `AdminController` - Admin operations
- `SecureController` - General protected endpoints

**Design Principles:**
- Thin controllers (business logic in services)
- Single Responsibility Principle
- Dependency Injection

### 2. Business Logic Layer (Services)

**Location:** `Services/`

**Responsibilities:**
- Token generation and validation
- Business rules implementation
- Data transformation

**Components:**
- `IJwtTokenService` - Interface for JWT operations
- `JwtTokenService` - JWT token generation implementation

**Design Patterns:**
- **Dependency Inversion Principle** - Interfaces define contracts
- **Service Pattern** - Encapsulates business logic
- **Factory Pattern** - Token creation

### 3. Data Access Layer

**Location:** `Data/`

**Responsibilities:**
- Database context management
- Entity configuration
- Data seeding

**Components:**
- `AppDbContext` - EF Core DbContext
- `SeedData` - Initial data seeding

**Design Patterns:**
- **Repository Pattern** (via EF Core)
- **Unit of Work Pattern** (via DbContext)

### 4. Domain Models

**Location:** `Model/`

**Responsibilities:**
- Domain entity definitions
- Business object representation

**Components:**
- `RefreshToken` - Refresh token model

## Design Patterns

### 1. Dependency Injection (DI)

**Implementation:**
- Built-in ASP.NET Core DI container
- Constructor injection throughout
- Service lifetime management (Scoped, Singleton, Transient)

**Example:**
```csharp
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
```

### 2. Repository Pattern

**Implementation:**
- Entity Framework Core acts as repository abstraction
- DbContext provides data access abstraction
- No explicit repository classes (EF Core is the repository)

### 3. Strategy Pattern

**Implementation:**
- JWT authentication scheme selection
- Multiple authentication strategies support
- Token validation strategies

### 4. Factory Pattern

**Implementation:**
- Token generation factory methods
- Refresh token creation

### 5. Middleware Pattern

**Implementation:**
- Authentication middleware
- Authorization middleware
- Request pipeline processing

## Data Flow

### Authentication Flow

```
1. Client Request
   ↓
2. AuthController.Login()
   ↓
3. UserManager.FindByEmailAsync()
   ↓
4. SignInManager.CheckPasswordSignInAsync()
   ↓
5. JwtTokenService.GenerateToken()
   ↓
6. Token stored in HttpOnly cookie + returned in response
   ↓
7. Client receives token
```

### Authorization Flow

```
1. Client Request with Token
   ↓
2. JWT Bearer Middleware validates token
   ↓
3. Claims extracted from token
   ↓
4. Authorization Middleware checks roles
   ↓
5. Controller action executed (if authorized)
   ↓
6. Response returned
```

### Token Generation Flow

```
1. User credentials validated
   ↓
2. User roles retrieved
   ↓
3. Claims created (Name, NameIdentifier, Roles)
   ↓
4. Signing credentials created (HMAC SHA256)
   ↓
5. JWT token generated with expiration
   ↓
6. Token serialized to string
```

## Technology Decisions

### Why ASP.NET Core 8.0?

- **Performance**: High-performance, cross-platform framework
- **Modern Features**: Latest C# language features
- **Long-term Support**: LTS version with extended support
- **Ecosystem**: Rich package ecosystem

### Why JWT Bearer Authentication?

- **Stateless**: No server-side session storage
- **Scalable**: Works across multiple servers
- **Standard**: Industry-standard (RFC 7519)
- **Flexible**: Can include custom claims

### Why ASP.NET Core Identity?

- **Mature**: Battle-tested user management
- **Secure**: Built-in password hashing (PBKDF2)
- **Extensible**: Easy to customize
- **Integrated**: Works seamlessly with EF Core

### Why Entity Framework Core?

- **Productivity**: Reduces boilerplate code
- **LINQ**: Type-safe queries
- **Migrations**: Database versioning
- **Cross-platform**: Works on all platforms

### Why SQL Server?

- **Enterprise-grade**: Reliable and scalable
- **Feature-rich**: Advanced security features
- **Integration**: Works well with .NET ecosystem
- **Flexibility**: Can be replaced with other providers

## Industry Standards Compliance

### RESTful API Design

✅ **Compliant:**
- Resource-based URLs (`/api/auth/login`)
- HTTP verbs (GET, POST)
- Stateless communication
- JSON response format

### OAuth 2.0 / JWT Standards

✅ **Compliant:**
- JWT token structure (Header.Payload.Signature)
- Standard claims (iss, aud, exp, sub)
- HMAC SHA256 signing algorithm
- Token expiration

### Security Standards

✅ **Compliant:**
- HTTPS enforcement (Secure flag)
- HttpOnly cookies (XSS protection)
- SameSite protection (CSRF protection)
- Password hashing (PBKDF2)

### API Documentation

✅ **Compliant:**
- OpenAPI/Swagger specification
- Endpoint documentation
- Request/response schemas

## Current Architecture Strengths

1. **Separation of Concerns**: Clear layer boundaries
2. **Dependency Injection**: Loose coupling
3. **Interface-based Design**: Testable and extensible
4. **Standard Patterns**: Follows .NET conventions
5. **Security First**: Built-in security features

## Architecture Improvements Needed

### 1. Repository Pattern Implementation

**Current:** Direct DbContext usage
**Recommended:** Explicit repository interfaces

```csharp
public interface IUserRepository
{
    Task<IdentityUser?> GetByEmailAsync(string email);
    Task<bool> CreateAsync(IdentityUser user, string password);
}
```

### 2. DTO Pattern

**Current:** Direct model usage in controllers
**Recommended:** Data Transfer Objects

```csharp
public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    [MinLength(8)]
    public string Password { get; set; }
}
```

### 3. Response Wrapper

**Current:** Direct object returns
**Recommended:** Standardized API responses

```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }
}
```

### 4. Exception Handling Middleware

**Current:** Default exception handling
**Recommended:** Global exception handler

```csharp
public class GlobalExceptionMiddleware
{
    // Centralized error handling
    // Logging
    // Standardized error responses
}
```

### 5. CQRS Pattern (Optional)

**For Complex Applications:**
- Separate read and write operations
- Command handlers for mutations
- Query handlers for reads

### 6. Unit of Work Pattern

**Current:** Direct DbContext usage
**Recommended:** Explicit Unit of Work

```csharp
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
}
```

## Scalability Considerations

### Horizontal Scaling

- **Stateless Design**: JWT tokens enable stateless authentication
- **Load Balancing**: No session affinity required
- **Database**: Consider read replicas for scaling

### Vertical Scaling

- **Async/Await**: Non-blocking I/O operations
- **Connection Pooling**: EF Core connection pooling
- **Caching**: Consider Redis for token blacklisting

### Microservices Readiness

- **Service Boundaries**: Clear service interfaces
- **API Gateway**: Can be fronted by API Gateway
- **Service Discovery**: Can integrate with service mesh

## Performance Optimizations

### Current Optimizations

1. **Async/Await**: Non-blocking operations
2. **Connection Pooling**: EF Core default
3. **Compiled Queries**: EF Core optimization

### Recommended Optimizations

1. **Response Caching**: Cache static data
2. **Output Caching**: Cache API responses
3. **Database Indexing**: Optimize queries
4. **Token Caching**: Cache user roles/claims

## Monitoring and Observability

### Recommended Additions

1. **Logging**: Structured logging (Serilog)
2. **Metrics**: Application Insights or Prometheus
3. **Tracing**: Distributed tracing
4. **Health Checks**: Endpoint health monitoring

## Conclusion

The current architecture provides a solid foundation with:
- Clear separation of concerns
- Industry-standard patterns
- Security-first approach
- Extensibility

Recommended improvements focus on:
- Enhanced testability
- Better error handling
- Standardized responses
- Production-ready features

See [FUTURE_IMPROVEMENTS.md](./FUTURE_IMPROVEMENTS.md) for detailed enhancement roadmap.


