using BudgetPlaner.UI.ApiClients.Identity;
using BudgetPlaner.UI.ApiClients;
using BudgetPlaner.UI.Components;
using BudgetPlaner.UI.Authentication;
using BudgetPlaner.UI.Services;
using BudgetPlaner.UI.Infrastructure;
using BudgetPlaner.Contracts.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddCircuitOptions(options => { options.DetailedErrors = true; });

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorizationCore();
builder.Services.AddMudServices();

builder.Services.AddHttpClient<IIdentityService, IdentityService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["BaseUrl"]!);
});

builder.Services.AddHttpClient<ICategoryService, CategoryService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["BaseUrl"]!);
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddHttpMessageHandler<AuthTokenHandler>();

builder.Services.AddTransient<AuthTokenHandler>();

builder.Services.AddHttpClient<ITokenRefreshService, TokenRefreshService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5271"); 
    client.Timeout = TimeSpan.FromSeconds(30);
});


builder.Services.AddScoped<AuthenticationStateProvider, UserRevalidatingState>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
{
    o.LoginPath = "/login";
    o.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    o.Events = new CookieAuthenticationEvents
    {
        OnValidatePrincipal = ctx =>
        {
            if (!(ctx.Principal?.Identity?.IsAuthenticated ?? false)) return Task.CompletedTask;
            var claims = ctx.Principal?.Claims;
            if (claims != null && claims.Any()) return Task.CompletedTask;
            ctx.RejectPrincipal();
            return ctx.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    };
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

// Add debug logging for requests
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogDebug("Request: {Method} {Path}", context.Request.Method, context.Request.Path);
    await next(context);
});

app.MapPost("/api/auth/refresh", async (HttpContext context, IIdentityService identityService, ILogger<Program> logger) =>
{
    logger.LogDebug("Token refresh endpoint called");
    try
    {
        // Get refresh token from claims or auth properties
        var refreshToken = context.User.FindFirst(BudgetPlaner.Contracts.Claims.BudgetPlanerClaims.RefreshToken)?.Value;
        if (string.IsNullOrEmpty(refreshToken))
        {
            refreshToken = await context.GetTokenAsync("refresh_token");
        }

        if (string.IsNullOrEmpty(refreshToken))
        {
            logger.LogWarning("No refresh token available");
            return Results.BadRequest(new { error = "No refresh token available" });
        }

        // Call the identity service to refresh the token
        var tokenResponse = await identityService.RefreshToken(refreshToken);
        if (tokenResponse == null)
        {
            logger.LogWarning("Token refresh failed");
            return Results.BadRequest(new { error = "Token refresh failed" });
        }

        // Create new principal with updated tokens
        var principal = BudgetPlaner.UI.Services.SignInService.GetPrincipal(tokenResponse);
        
        // Create authentication properties with new tokens
        var authProperties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties
        {
            IsPersistent = context.User.Identity?.AuthenticationType == Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme,
            ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
        };
        
        // Store tokens in authentication properties as well
        authProperties.StoreTokens(new[]
        {
            new Microsoft.AspNetCore.Authentication.AuthenticationToken { Name = "access_token", Value = tokenResponse.AccessToken! },
            new Microsoft.AspNetCore.Authentication.AuthenticationToken { Name = "refresh_token", Value = tokenResponse.RefreshToken! },
            new Microsoft.AspNetCore.Authentication.AuthenticationToken { Name = "token_type", Value = tokenResponse.TokenType ?? "Bearer" },
            new Microsoft.AspNetCore.Authentication.AuthenticationToken { Name = "expires_at", Value = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn).ToString("o") }
        });
        
        // Sign in with new tokens
        await context.SignInAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);
        
        return Results.Ok(new { success = true, accessToken = tokenResponse.AccessToken });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error during token refresh");
        return Results.Problem(
            detail: "Internal server error during token refresh",
            statusCode: 500);
    }
}).RequireAuthorization();

// Add a simple test endpoint to verify routing
app.MapGet("/api/test", () => Results.Ok(new { message = "API routing is working!" }));

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
