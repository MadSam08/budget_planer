using BudgetPlaner.UI.ApiClients.Identity;
using BudgetPlaner.UI.ApiClients;
using BudgetPlaner.UI.Components;
using BudgetPlaner.UI.Authentication;
using BudgetPlaner.UI.Services;
using BudgetPlaner.UI.Infrastructure;
using BudgetPlaner.Sdk;
using BudgetPlaner.Sdk.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using Refit;

var builder = WebApplication.CreateBuilder(args);

// Configure JSON serialization for consistent handling
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});

// Configure JSON serialization for Refit/HttpClient
builder.Services.AddHttpClient().ConfigureHttpClientDefaults(httpClientBuilder =>
{
    httpClientBuilder.ConfigureHttpClient((serviceProvider, httpClient) =>
    {
        httpClient.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    });
});

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


// Register the AuthTokenHandler for authentication
builder.Services.AddTransient<AuthTokenHandler>();

builder.Services
    .AddRefitClient<ICategoriesApi>()
    .ConfigureHttpClient(x => x.BaseAddress = new Uri(builder.Configuration["BaseUrl"]!))
    .AddHttpMessageHandler<AuthTokenHandler>();

builder.Services
    .AddRefitClient<IIncomesApi>()
    .ConfigureHttpClient(x => x.BaseAddress = new Uri(builder.Configuration["BaseUrl"]!))
    .AddHttpMessageHandler<AuthTokenHandler>();

builder.Services
    .AddRefitClient<IExpensesApi>()
    .ConfigureHttpClient(x => x.BaseAddress = new Uri(builder.Configuration["BaseUrl"]!))
    .AddHttpMessageHandler<AuthTokenHandler>();

builder.Services
    .AddRefitClient<ILoansApi>()
    .ConfigureHttpClient(x => x.BaseAddress = new Uri(builder.Configuration["BaseUrl"]!))
    .AddHttpMessageHandler<AuthTokenHandler>();

builder.Services
    .AddRefitClient<IBudgetsApi>()
    .ConfigureHttpClient(x => x.BaseAddress = new Uri(builder.Configuration["BaseUrl"]!))
    .AddHttpMessageHandler<AuthTokenHandler>();

builder.Services
    .AddRefitClient<IInsightsApi>()
    .ConfigureHttpClient(x => x.BaseAddress = new Uri(builder.Configuration["BaseUrl"]!))
    .AddHttpMessageHandler<AuthTokenHandler>();

builder.Services
    .AddRefitClient<ICurrenciesApi>()
    .ConfigureHttpClient(x => x.BaseAddress = new Uri(builder.Configuration["BaseUrl"]!))
    .AddHttpMessageHandler<AuthTokenHandler>();

builder.Services
    .AddRefitClient<IAuthApi>()
    .ConfigureHttpClient(x => x.BaseAddress = new Uri(builder.Configuration["BaseUrl"]!))
    .AddHttpMessageHandler<AuthTokenHandler>();

builder.Services.AddScoped<IBudgetPlanerClient, BudgetPlanerClient>();

builder.Services.AddScoped<IBudgetPlanerSdkService, BudgetPlanerSdkService>();
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

app.MapPost("/api/auth/refresh",
    async (HttpContext context, IIdentityService identityService, ILogger<Program> logger) =>
    {
        logger.LogDebug("Token refresh endpoint called");
        try
        {
            // Get refresh token from claims or auth properties
            var refreshToken = context.User.FindFirst(BudgetPlaner.Contracts.Claims.BudgetPlanerClaims.RefreshToken)
                ?.Value;
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
                IsPersistent = context.User.Identity?.AuthenticationType == Microsoft.AspNetCore.Authentication
                    .Cookies.CookieAuthenticationDefaults.AuthenticationScheme,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
            };

            // Store tokens in authentication properties as well
            authProperties.StoreTokens(new[]
            {
                new Microsoft.AspNetCore.Authentication.AuthenticationToken
                    { Name = "access_token", Value = tokenResponse.AccessToken! },
                new Microsoft.AspNetCore.Authentication.AuthenticationToken
                    { Name = "refresh_token", Value = tokenResponse.RefreshToken! },
                new Microsoft.AspNetCore.Authentication.AuthenticationToken
                    { Name = "token_type", Value = tokenResponse.TokenType ?? "Bearer" },
                new Microsoft.AspNetCore.Authentication.AuthenticationToken
                {
                    Name = "expires_at", Value = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn).ToString("o")
                }
            });

            // Sign in with new tokens
            await context.SignInAsync(
                Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme,
                principal, authProperties);

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

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
