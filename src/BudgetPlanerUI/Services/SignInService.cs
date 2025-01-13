using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BudgetPlaner.Contracts.Api;
using BudgetPlaner.Contracts.Api.Identity;
using BudgetPlaner.Contracts.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BudgetPlaner.UI.Services;

public class SignInService
{
    public static ClaimsPrincipal GetPrincipal(TokenResponse tokenResponse)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.AuthenticationInstant, DateTime.UtcNow.Ticks.ToString()),
            new(BudgetPlanerClaims.AccessToken, tokenResponse.AccessToken!),
            new(BudgetPlanerClaims.RefreshToken, tokenResponse.RefreshToken!),
            new(BudgetPlanerClaims.ExpiresIn, tokenResponse.ExpiresIn.ToString(), ClaimValueTypes.Integer),
        };

        var userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        return new ClaimsPrincipal(userIdentity);
    }
}