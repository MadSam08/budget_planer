using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BudgetPlaner.Contracts.Api;
using BudgetPlaner.Contracts.Api.Identity;
using BudgetPlaner.UI.ApiClients.Identity;
using BudgetPlaner.UI.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;

namespace BudgetPlaner.UI.Authentication;

// This is a server-side AuthenticationStateProvider that revalidates the security stamp for the connected user
// every 30 minutes an interactive circuit is connected.

internal sealed class UserRevalidatingState(
    ILoggerFactory loggerFactory, 
    IIdentityService identityService,
    IHttpContextAccessor httpContextAccessor)
    : RevalidatingServerAuthenticationStateProvider(loggerFactory)
{
    protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);

    protected override async Task<bool> ValidateAuthenticationStateAsync(
        AuthenticationState authenticationState, CancellationToken cancellationToken)
    {
        var user = authenticationState.User;

        if (!(user?.Identity?.IsAuthenticated ?? false)) return false;

        var accessToken = await GetCookieValueAsync("accessToken");
        if (string.IsNullOrEmpty(accessToken)) return false;

        var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
        var expiryDate = jwtToken.ValidTo;

        if (expiryDate > DateTime.UtcNow) return true;
        
        var refreshToken = await GetCookieValueAsync("refreshToken");
        if (string.IsNullOrEmpty(refreshToken)) return false;

        var newTokens = await RefreshTokensAsync(refreshToken);
        if (newTokens == null) return false;

        SetCookie("accessToken", newTokens.AccessToken!);
        SetCookie("refreshToken", newTokens.RefreshToken!);

        var newUser = SignInService.GetPrincipal(newTokens);
        var newAuthenticationState = Task.FromResult(new AuthenticationState(newUser));
        NotifyAuthenticationStateChanged(newAuthenticationState);

        return true;
    }

    private async Task<TokenResponse?> RefreshTokensAsync(string? refreshToken)
    {
        var response = await identityService.RefreshToken(refreshToken);
        return response ?? null;
    }

    private async Task<string?> GetCookieValueAsync(string key)
    {
        var cookie = httpContextAccessor.HttpContext?.Request.Cookies[key];
        return await Task.FromResult(cookie);
    }

    private void SetCookie(string key, string value)
    {
        var cookies = httpContextAccessor.HttpContext?.Response.Cookies;
        cookies?.Append(key, value, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(60)
        });
    }
}