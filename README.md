# 🔐 JWT Authentication System (.NET 8 Web API)

This project demonstrates a **production-ready authentication system**
built using **ASP.NET Core 8**, **JWT**, and **ASP.NET Identity**.

---

## 🚀 Features

- JWT-based authentication
- Refresh token support
- Role-based authorization (Admin / User)
- ASP.NET Identity integration
- HttpOnly cookie-based token storage
- Secure login & logout
- Protected APIs using `[Authorize]`

---

## 🏗 Tech Stack

- .NET 8 Web API
- ASP.NET Identity
- Entity Framework Core
- SQL Server
- JWT (JSON Web Tokens)

---

## 🔐 Authentication Flow

1. User registers using `/api/auth/register`
2. User logs in using `/api/auth/login`
3. JWT Access Token is issued
4. Token is stored in **HttpOnly Cookie**
5. Protected APIs validate token automatically
6. User logs out using `/api/auth/logout`

---

## 🛡 Security Implementation

- Passwords are hashed using ASP.NET Identity
- Access tokens are short-lived
- Refresh tokens are used for session continuity
- JWT stored in HttpOnly cookies to prevent XSS
- Role-based API protection

---

## 🔑 Authorization Examples

```csharp
[Authorize]
[Authorize(Roles = "Admin")]
