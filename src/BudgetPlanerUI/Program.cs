using BudgetPlaner.UI.ApiClients.Identity;
using BudgetPlaner.UI.ApiClients;
using BudgetPlaner.UI.Components;
using BudgetPlaner.UI.Authentication;
using BudgetPlaner.UI.Extensions;
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

// Register identity service (no authentication needed)
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
builder.Services.AddAuthenticatedHttpClient<IUserProfileService, UserProfileService>(builder.Configuration);

// Register custom authentication state provider
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

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
