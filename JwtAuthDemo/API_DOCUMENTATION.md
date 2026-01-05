# API Documentation

Complete API reference for the JWT Authentication Demo application.

## Table of Contents

- [Base URL](#base-url)
- [Authentication](#authentication)
- [Endpoints](#endpoints)
- [Error Responses](#error-responses)
- [Rate Limiting](#rate-limiting)
- [Examples](#examples)

## Base URL

**Development:**
- HTTP: `http://localhost:5225`
- HTTPS: `https://localhost:7093`

**Production:**
- Configure based on deployment environment

## Authentication

The API uses JWT (JSON Web Token) Bearer authentication. Tokens are provided in two ways:

1. **Authorization Header:**
   ```
   Authorization: Bearer {accessToken}
   ```

2. **HttpOnly Cookie:**
   - Automatically set on login
   - Sent with each request
   - HttpOnly, Secure, SameSite=Strict

### Token Structure

**Access Token:**
- **Type:** JWT
- **Algorithm:** HS256 (HMAC SHA-256)
- **Expiration:** 15 minutes (configurable)
- **Claims:**
  - `sub` (Subject): Username
  - `nameid` (Name Identifier): User ID
  - `role`: User roles (Admin, User)
  - `iss` (Issuer): JwtAuthDemo
  - `aud` (Audience): JwtAuthDemoUsers
  - `exp` (Expiration): Unix timestamp
  - `iat` (Issued At): Unix timestamp

**Refresh Token:**
- **Type:** Base64-encoded random string
- **Expiration:** 7 days (configurable)
- **Storage:** Database (to be implemented)

## Endpoints

### Authentication Endpoints

#### Register User

Creates a new user account.

```http
POST /api/auth/register
Content-Type: application/x-www-form-urlencoded
```

**Parameters:**
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| email | string | Yes | User email address |
| password | string | Yes | User password (min 6 chars) |

**Request Example:**
```http
POST /api/auth/register
Content-Type: application/x-www-form-urlencoded

email=user@example.com&password=SecurePass123!
```

**Success Response (200 OK):**
```json
{
  "message": "User registered successfully"
}
```

**Error Response (400 Bad Request):**
```json
{
  "errors": [
    {
      "code": "DuplicateUserName",
      "description": "User name 'user@example.com' is already taken."
    }
  ]
}
```

**Possible Error Codes:**
- `DuplicateUserName` - Email already registered
- `PasswordTooShort` - Password doesn't meet requirements
- `InvalidEmail` - Invalid email format

---

#### Login

Authenticates a user and returns access and refresh tokens.

```http
POST /api/auth/login
Content-Type: application/x-www-form-urlencoded
```

**Parameters:**
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| email | string | Yes | User email address |
| password | string | Yes | User password |

**Request Example:**
```http
POST /api/auth/login
Content-Type: application/x-www-form-urlencoded

email=user@example.com&password=SecurePass123!
```

**Success Response (200 OK):**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ1c2VyQGV4YW1wbGUuY29tIiwibmFtZWlkIjoiMTIzNDU2Iiwicm9sZSI6IlVzZXIiLCJleHAiOjE2OTk5OTk5OTksImlhdCI6MTY5OTk5OTk5OX0.signature",
  "refreshToken": "base64-encoded-refresh-token-string"
}
```

**Note:** The access token is also set as an HttpOnly cookie named `access_token`.

**Error Response (401 Unauthorized):**
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401,
  "traceId": "00-1234567890abcdef-1234567890abcdef-01"
}
```

**Possible Error Scenarios:**
- Invalid email or password
- User account not found
- Account locked (if lockout enabled)

---

#### Refresh Token

Generates a new access token using a valid refresh token.

```http
POST /api/auth/refresh
Content-Type: application/x-www-form-urlencoded
```

**Parameters:**
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| refreshToken | string | Yes | Valid refresh token |

**Request Example:**
```http
POST /api/auth/refresh
Content-Type: application/x-www-form-urlencoded

refreshToken=base64-encoded-refresh-token-string
```

**Success Response (200 OK):**
```json
{
  "accessToken": "new-access-token-here"
}
```

**Error Response (401 Unauthorized):**
```json
{
  "message": "Invalid refresh token"
}
```

**Note:** This endpoint is currently incomplete. See [SECURITY.md](./SECURITY.md) for implementation details.

---

### Protected Endpoints

All protected endpoints require a valid JWT token in the Authorization header or as an HttpOnly cookie.

#### Get Secure Data

Returns a simple authorized message. Requires any authenticated user.

```http
GET /api/secure
Authorization: Bearer {accessToken}
```

**Request Example:**
```http
GET /api/secure
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Success Response (200 OK):**
```json
"You are authorized!"
```

**Error Response (401 Unauthorized):**
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401
}
```

---

#### Get User Profile

Returns user profile data. Requires `User` role.

```http
GET /api/User/profile
Authorization: Bearer {accessToken}
```

**Request Example:**
```http
GET /api/User/profile
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Success Response (200 OK):**
```json
"User profile data"
```

**Error Response (401 Unauthorized):**
- Missing or invalid token

**Error Response (403 Forbidden):**
- Valid token but insufficient permissions (missing `User` role)

---

#### Get Admin Dashboard

Returns admin dashboard data. Requires `Admin` role.

```http
GET /api/admin/dashboard
Authorization: Bearer {accessToken}
```

**Request Example:**
```http
GET /api/admin/dashboard
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Success Response (200 OK):**
```json
"Admin dashboard data"
```

**Error Response (401 Unauthorized):**
- Missing or invalid token

**Error Response (403 Forbidden):**
- Valid token but insufficient permissions (missing `Admin` role)

**Note:** Currently, the AdminController has `[Authorize(Roles = "User")]` which appears to be incorrect. It should be `[Authorize(Roles = "Admin")]`.

---

## Error Responses

### Standard Error Format

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "00-1234567890abcdef-1234567890abcdef-01",
  "errors": {
    "email": [
      "The email field is required."
    ]
  }
}
```

### HTTP Status Codes

| Code | Meaning | Description |
|------|---------|-------------|
| 200 | OK | Request successful |
| 400 | Bad Request | Invalid request parameters |
| 401 | Unauthorized | Missing or invalid authentication |
| 403 | Forbidden | Insufficient permissions |
| 404 | Not Found | Resource not found |
| 500 | Internal Server Error | Server error |

### Common Error Scenarios

#### Invalid Token
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401
}
```

#### Expired Token
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401,
  "detail": "Token has expired"
}
```

#### Insufficient Permissions
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.3",
  "title": "Forbidden",
  "status": 403
}
```

## Rate Limiting

**Current Status:** Not implemented

**Planned Implementation:**
- Login endpoint: 5 requests per minute
- Register endpoint: 3 requests per hour
- Refresh endpoint: 10 requests per minute

When rate limit is exceeded, the API will return:
```json
{
  "type": "https://tools.ietf.org/html/rfc6585#section-4",
  "title": "Too Many Requests",
  "status": 429,
  "detail": "Rate limit exceeded. Please try again later."
}
```

## Examples

### Complete Authentication Flow

#### 1. Register a New User

```bash
curl -X POST "https://localhost:7093/api/auth/register" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "email=newuser@example.com&password=SecurePass123!" \
  -k
```

#### 2. Login

```bash
curl -X POST "https://localhost:7093/api/auth/login" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "email=newuser@example.com&password=SecurePass123!" \
  -c cookies.txt \
  -k
```

Response:
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "refresh-token-here"
}
```

#### 3. Access Protected Endpoint

```bash
curl -X GET "https://localhost:7093/api/secure" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -b cookies.txt \
  -k
```

#### 4. Refresh Access Token

```bash
curl -X POST "https://localhost:7093/api/auth/refresh" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "refreshToken=refresh-token-here" \
  -k
```

### Using JavaScript (Fetch API)

#### Login Example

```javascript
async function login(email, password) {
  const formData = new URLSearchParams();
  formData.append('email', email);
  formData.append('password', password);
  
  const response = await fetch('https://localhost:7093/api/auth/login', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/x-www-form-urlencoded',
    },
    body: formData,
    credentials: 'include' // Include cookies
  });
  
  if (response.ok) {
    const data = await response.json();
    localStorage.setItem('accessToken', data.accessToken);
    localStorage.setItem('refreshToken', data.refreshToken);
    return data;
  } else {
    throw new Error('Login failed');
  }
}
```

#### Access Protected Endpoint

```javascript
async function getSecureData() {
  const token = localStorage.getItem('accessToken');
  
  const response = await fetch('https://localhost:7093/api/secure', {
    method: 'GET',
    headers: {
      'Authorization': `Bearer ${token}`,
    },
    credentials: 'include'
  });
  
  if (response.ok) {
    const data = await response.text();
    return data;
  } else if (response.status === 401) {
    // Token expired, try refresh
    await refreshToken();
    return getSecureData(); // Retry
  } else {
    throw new Error('Request failed');
  }
}
```

### Using Postman

1. **Create Environment Variables:**
   - `baseUrl`: `https://localhost:7093`
   - `accessToken`: (will be set after login)
   - `refreshToken`: (will be set after login)

2. **Register Request:**
   - Method: `POST`
   - URL: `{{baseUrl}}/api/auth/register`
   - Body: `x-www-form-urlencoded`
   - Fields:
     - `email`: `test@example.com`
     - `password`: `Test123!`

3. **Login Request:**
   - Method: `POST`
   - URL: `{{baseUrl}}/api/auth/login`
   - Body: `x-www-form-urlencoded`
   - Fields:
     - `email`: `test@example.com`
     - `password`: `Test123!`
   - Tests (to save token):
     ```javascript
     if (pm.response.code === 200) {
       const jsonData = pm.response.json();
       pm.environment.set("accessToken", jsonData.accessToken);
       pm.environment.set("refreshToken", jsonData.refreshToken);
     }
     ```

4. **Protected Endpoint:**
   - Method: `GET`
   - URL: `{{baseUrl}}/api/secure`
   - Authorization: `Bearer Token`
   - Token: `{{accessToken}}`

## Swagger/OpenAPI

The API includes Swagger/OpenAPI documentation available at:

**Development:**
- Swagger UI: `https://localhost:7093/swagger`
- OpenAPI JSON: `https://localhost:7093/swagger/v1/swagger.json`

### Using Swagger UI

1. Navigate to `/swagger` when the app is running
2. Click "Authorize" button
3. Enter: `Bearer {your-token-here}`
4. Click "Authorize"
5. Test endpoints directly from the UI

## Best Practices

### 1. Token Storage
- **Server-side:** Use HttpOnly cookies (implemented)
- **Client-side:** Store in memory, not localStorage (for SPAs)
- **Mobile:** Use secure storage (Keychain/Keystore)

### 2. Token Refresh
- Refresh token before expiration
- Handle 401 responses by refreshing token
- Implement exponential backoff for retries

### 3. Error Handling
- Check response status codes
- Handle network errors gracefully
- Provide user-friendly error messages

### 4. Security
- Always use HTTPS in production
- Never log tokens
- Implement token rotation
- Revoke tokens on logout

## Versioning

**Current Version:** 1.0

Future versions will be indicated in the URL:
- `/api/v1/auth/login`
- `/api/v2/auth/login`

## Support

For issues, questions, or contributions, please refer to:
- [README.md](./README.md) - Project overview
- [SECURITY.md](./SECURITY.md) - Security documentation
- [ARCHITECTURE.md](./ARCHITECTURE.md) - Architecture details


