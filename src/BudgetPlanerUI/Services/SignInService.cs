using System.Security.Claims;
using BudgetPlaner.Contracts.Api.Identity;
using BudgetPlaner.Contracts.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BudgetPlaner.UI.Services;

public class SignInService
{
    public static ClaimsPrincipal GetPrincipal(TokenResponse tokenResponse)
    {
        // Calculate the actual expiry time
        var expiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);
        
        var claims = new List<Claim>
        {
            new(BudgetPlanerClaims.AccessToken, tokenResponse.AccessToken!),
            new(BudgetPlanerClaims.RefreshToken, tokenResponse.RefreshToken!),
            new(BudgetPlanerClaims.ExpiresIn, tokenResponse.ExpiresIn.ToString(), ClaimValueTypes.Integer),
            new(BudgetPlanerClaims.ExpiresAt, expiresAt.ToString("O"), ClaimValueTypes.DateTime), // ISO 8601 format
            new(ClaimTypes.AuthenticationInstant, DateTime.UtcNow.Ticks.ToString())
        };

        var userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        return new ClaimsPrincipal(userIdentity);
    }
}