# Configuration Guide

## Environment Setup

### Development Environment

1. **JWT Secret Configuration**
   - The JWT secret is configured in `appsettings.Development.json`
   - Current dev secret: `"dev-super-secret-key-that-is-at-least-32-characters-long!!!"`
   - This is only for development; change before production use

2. **Database Connection**
   - Current connection string in `appsettings.json`:
     ```
     Server=(local)\SQLEXPRESS;Database=SalesQuotationDb;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;
     ```
   - Update this to your local SQL Server instance

### Production Environment

1. **JWT Secret Configuration (IMPORTANT)**
   - Set environment variable: `JWT_SECRET_KEY` with a minimum 32-character random key
   - Example: `$env:JWT_SECRET_KEY = "your-secure-32-plus-character-random-key-here"`
   - The application will validate and throw an error if not properly configured

2. **Database Connection**
   - Update `appsettings.json` with your production database connection string
   - Use secure credentials (not hardcoded)
   - Consider using Azure Key Vault or similar for connection strings

3. **Logging**
   - Serilog is configured to write logs to:
     - Console output
     - Daily rotating file: `logs/app-{date}.txt`
   - Ensure application has write permissions to the logs directory

4. **CORS Policy**
   - Currently set to `AllowAll` for development
   - Update in `Program.cs` for production:
     ```csharp
     options.AddPolicy("AllowSpecific", policy =>
     {
         policy.WithOrigins("https://yourdomain.com")
               .AllowAnyMethod()
               .AllowAnyHeader();
     });
     ```

## API Usage Examples

### 1. Login Endpoint
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "password123"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "user": {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "name": "John Doe",
      "email": "user@example.com",
      "phone": "+1234567890",
      "role": "Staff",
      "isActive": true,
      "createdAt": "2024-01-15T10:30:00Z"
    },
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
  }
}
```

**Error Response (401 Unauthorized):**
```json
{
  "success": false,
  "error": "Invalid credentials",
  "code": "UNAUTHORIZED"
}
```

### 2. Logout Endpoint
```http
POST /api/auth/logout
Authorization: Bearer {token}
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "message": "Logged out successfully"
  }
}
```

## Middleware Pipeline

The application uses the following middleware stack (in order):

1. **Global Exception Handler** - Catches and formats all exceptions
2. **HTTPS Redirect** - Enforces HTTPS in non-development environments
3. **CORS** - Handles cross-origin requests
4. **Authentication** - JWT validation
5. **Authorization** - Endpoint permission checking
6. **Routing & Controllers** - API endpoints

## Database Migrations

To apply database migrations:

```bash
# Add migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# View pending migrations
dotnet ef migrations list
```

Migrations are applied automatically on application startup in the `Program.cs` boot code.

## Logging Configuration

### Log Levels
- **Debug**: Detailed diagnostic information
- **Information**: General informational messages (default)
- **Warning**: Warning messages for potentially harmful situations
- **Error**: Error messages for serious problems

### Configuration
Current configuration in `Program.cs`:
```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/app-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
```

## Validation

### FluentValidation Rules

**LoginRequestDto Validation:**
- Email: Required, valid email format
- Password: Required, minimum 6 characters

**CreateEnquiryDto Validation:**
- CustomerName: Required
- CustomerPhone: Required
- CustomerEmail: Optional but must be valid if provided

Add more validators in `SalesQuotation.Application/Validators/` folder.

## Security Headers

Consider adding these security headers in production:

```csharp
app.UseSecurityHeaders(policies =>
    policies.AddDefaultSecurityHeaders()
        .AddStrictTransportSecurityMaxAge(days: 365)
        .AddX-ContentTypeOptions()
        .AddX-Frame-Options()
        .AddX-XSS-Protection()
        .AddReferrerPolicy(ReferrerPolicy.StrictOriginWhenCrossOrigin));
```

## API Documentation

Swagger/OpenAPI documentation is available at:
- **Development**: `https://localhost:5001` (redirected from root)
- **Production**: `https://yourdomain.com/swagger/index.html`

The Swagger UI provides interactive API documentation and allows testing endpoints directly.

## Troubleshooting

### JWT Token Errors
- Ensure `JwtSettings:SecretKey` is at least 32 characters
- Check that token hasn't expired (default: 10,080 minutes = 7 days)
- Verify `[Authorize]` attributes on protected endpoints

### Database Connection Issues
- Verify SQL Server is running
- Check connection string in appsettings.json
- Ensure database exists or use EF Core migrations

### CORS Errors
- Check that client URL matches the CORS policy
- Ensure `Content-Type` header is properly set
- Verify preflight requests are not being blocked

## Performance Considerations

1. **Connection Pooling**: EF Core handles this automatically
2. **Logging**: Adjust log levels in production to reduce I/O
3. **Caching**: Consider adding distributed caching for frequently accessed data
4. **Pagination**: Implement for large result sets
5. **Async/Await**: All I/O operations use async patterns

## Monitoring

Implement application monitoring using:
- Application Insights (Azure)
- DataDog
- New Relic
- Prometheus + Grafana

Configure Serilog to send logs to your chosen monitoring service.
