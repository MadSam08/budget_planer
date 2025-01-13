using BudgetPlaner.UI.ApiClients.Identity;
using BudgetPlaner.UI.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddCircuitOptions(options => { options.DetailedErrors = true; });

builder.Services.AddHttpClient();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorizationCore();
builder.Services.AddMudServices();

builder.Services.AddHttpClient<IIdentityService, IdentityService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["BaseUrl"]!);
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
{
    o.LoginPath = "/account/sign-in";
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

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.MapStaticAssets();
app.UseAntiforgery();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
