# BudgetPlaner UI - Authentication System

## Overview

This project implements a comprehensive Bearer token authentication system with automatic token refresh for Blazor Server applications. The system handles JWT tokens stored in claims and automatically refreshes them when they expire.

## Architecture

### Key Components

1. **`ITokenProvider` & `TokenProvider`** - Service that safely accesses authentication tokens from HttpContext:
   - Provides access to access and refresh tokens
   - Validates token expiration before requests
   - Handles automatic token refresh
   - Works within Blazor Server DI scope limitations

2. **`AuthenticatedHttpMessageHandler`** - HTTP message handler that automatically:
   - Adds Bearer tokens to outgoing requests using ITokenProvider
   - Automatically refreshes expired tokens
   - Retries failed requests after token refresh
   - Thread-safe token refresh operations

3. **`BaseAuthenticatedService`** - Base class for all authenticated API services that provides:
   - Common HTTP operations (GET, POST, PUT, DELETE)
   - Consistent error handling and logging
   - JSON serialization configuration

4. **`ServiceCollectionExtensions`** - Extension methods to simplify service registration

## Authentication Flow

1. User logs in → JWT tokens stored in claims
2. API request initiated → TokenProvider gets valid access token
3. If token expired → TokenProvider refreshes token automatically
4. Request sent with valid Bearer token
5. If 401 response → Handler attempts token refresh and retries

## Blazor Server Compatibility Fix

**Previous Issue**: Using `AuthenticationStateProvider.GetAuthenticationStateAsync()` in HTTP message handlers caused:
```
System.InvalidOperationException: Do not call GetAuthenticationStateAsync outside of the DI scope for a Razor component.
```

**Solution**: Replaced `AuthenticationStateProvider` with `ITokenProvider` that uses `IHttpContextAccessor` to safely access user claims from any DI scope.

## Creating New Authenticated Services

### Step 1: Define Service Interface

```csharp
public interface IMyService
{
    Task<List<MyModel>> GetItemsAsync(CancellationToken cancellationToken = default);
    Task<MyModel?> GetItemAsync(string id, CancellationToken cancellationToken = default);
    Task<bool> CreateItemAsync(MyModel item, CancellationToken cancellationToken = default);
    // ... other methods
}
```

### Step 2: Implement Service

```csharp
public class MyService : BaseAuthenticatedService, IMyService
{
    private const string BasePath = "api/my-endpoint";

    public MyService(HttpClient client, ILogger<MyService> logger) 
        : base(client, logger)
    {
    }

    public async Task<List<MyModel>> GetItemsAsync(CancellationToken cancellationToken = default)
    {
        var result = await GetAsync<List<MyModel>>(BasePath, cancellationToken);
        return result ?? new List<MyModel>();
    }

    public async Task<MyModel?> GetItemAsync(string id, CancellationToken cancellationToken = default)
    {
        return await GetAsync<MyModel>($"{BasePath}/{id}", cancellationToken);
    }

    public async Task<bool> CreateItemAsync(MyModel item, CancellationToken cancellationToken = default)
    {
        return await PostAsync(BasePath, item, cancellationToken);
    }
}
```

### Step 3: Register Service

```csharp
// In Program.cs
builder.Services.AddAuthenticatedHttpClient<IMyService, MyService>(builder.Configuration);
```

## Features

### Automatic Token Management
- ✅ Bearer token automatically added to requests
- ✅ Token expiration validation (1-minute buffer)
- ✅ Automatic token refresh on expiration
- ✅ Automatic retry of failed requests after refresh
- ✅ Thread-safe token refresh (using semaphore)

### Error Handling
- ✅ Comprehensive logging for debugging
- ✅ Graceful fallback on authentication failures
- ✅ Automatic user sign-out on token refresh failure
- ✅ Consistent error responses across services

### Blazor Server Compatibility
- ✅ Works with server-side Blazor DI scope limitations
- ✅ Proper HttpContext handling via IHttpContextAccessor
- ✅ Cookie-based authentication state updates
- ✅ No AuthenticationStateProvider dependencies in handlers

### Performance Features
- ✅ Connection pooling via HttpClient factory
- ✅ Cancellation token support
- ✅ Minimal memory allocations
- ✅ Efficient JSON serialization

## Token Claims Structure

```csharp
public class BudgetPlanerClaims
{
    public const string AccessToken = "AccessToken";   // JWT access token
    public const string RefreshToken = "RefreshToken"; // Refresh token
    public const string ExpiresIn = "ExpiresIn";       // Token expiration time
}
```

## Configuration

### Required Configuration

```json
{
  "BaseUrl": "http://localhost:5240"  // API base URL
}
```

### Service Registration

```csharp
// Register identity service (no auth needed)
builder.Services.AddHttpClient<IIdentityService, IdentityService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["BaseUrl"]!);
});

// Register token provider service
builder.Services.AddScoped<ITokenProvider, TokenProvider>();

// Register the authenticated message handler
builder.Services.AddTransient<AuthenticatedHttpMessageHandler>();

// Register authenticated services
builder.Services.AddAuthenticatedHttpClient<ICategoryService, CategoryService>(builder.Configuration);
```

## Example: CategoryService Integration

The `CategoryService` demonstrates the complete implementation:

```csharp
[Inject] private ICategoryService CategoryService { get; set; } = null!;

// Usage in components - authentication handled automatically
var categories = await CategoryService.GetCategoriesAsync();
var success = await CategoryService.CreateCategoryAsync(newCategory);
```

## Security Features

- 🔒 Secure HttpOnly cookies for token storage
- 🔒 SameSite=Strict cookie policy
- 🔒 Automatic token expiration handling
- 🔒 Secure token refresh mechanism
- 🔒 Automatic cleanup on authentication failure

## Troubleshooting

### Common Issues

1. **401 Unauthorized Errors**: Check if the API endpoint requires authentication and tokens are properly configured
2. **Token Refresh Loops**: Verify refresh token endpoint is working and returns valid tokens
3. **DI Scope Errors**: Use ITokenProvider instead of AuthenticationStateProvider in services
4. **Memory Leaks**: Ensure proper disposal of HttpClient (handled by DI container)

### Logging

The system provides comprehensive logging:
- Token refresh attempts
- Authentication failures
- HTTP request/response details
- Error conditions with stack traces

### Debug Configuration

```csharp
builder.Logging.SetMinimumLevel(LogLevel.Debug);
```

This will show detailed authentication flow information in the console. 