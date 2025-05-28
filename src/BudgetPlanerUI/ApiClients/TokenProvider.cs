using System.IdentityModel.Tokens.Jwt;
using BudgetPlaner.Contracts.Claims;
using BudgetPlaner.UI.ApiClients.Identity;
using BudgetPlaner.UI.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BudgetPlaner.UI.ApiClients;

public class TokenProvider : ITokenProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;
    private readonly ILogger<TokenProvider> _logger;

    public TokenProvider(
        IHttpContextAccessor httpContextAccessor,
        IIdentityService identityService,
        ILogger<TokenProvider> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
        _logger = logger;
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        var accessToken = httpContext.User.FindFirst(BudgetPlanerClaims.AccessToken)?.Value;
        
        if (string.IsNullOrEmpty(accessToken))
        {
            return null;
        }

        // Check if token is expired
        if (IsTokenExpired(accessToken))
        {
            var refreshed = await RefreshTokenAsync();
            if (refreshed)
            {
                // Get the refreshed token
                var newHttpContext = _httpContextAccessor.HttpContext;
                return newHttpContext?.User.FindFirst(BudgetPlanerClaims.AccessToken)?.Value;
            }
            return null;
        }

        return accessToken;
    }

    public Task<string?> GetRefreshTokenAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated != true)
        {
            return Task.FromResult<string?>(null);
        }

        var refreshToken = httpContext.User.FindFirst(BudgetPlanerClaims.RefreshToken)?.Value;
        return Task.FromResult<string?>(refreshToken);
    }

    public async Task<bool> RefreshTokenAsync()
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return false;
            }

            var refreshToken = httpContext.User.FindFirst(BudgetPlanerClaims.RefreshToken)?.Value;
            
            if (string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogWarning("No refresh token found in user claims");
                return false;
            }

            var tokenResponse = await _identityService.RefreshToken(refreshToken);
            
            if (tokenResponse == null)
            {
                _logger.LogWarning("Failed to refresh access token");
                await SignOutUserAsync();
                return false;
            }

            // Update the authentication state with new tokens
            await UpdateAuthenticationStateAsync(tokenResponse);
            
            _logger.LogInformation("Access token refreshed successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while refreshing access token");
            await SignOutUserAsync();
            return false;
        }
    }

    private static bool IsTokenExpired(string token)
    {
        try
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            return jwtToken.ValidTo <= DateTime.UtcNow.AddMinutes(-1); // Add 1 minute buffer
        }
        catch
        {
            return true; // If we can't parse the token, consider it expired
        }
    }

    private async Task UpdateAuthenticationStateAsync(BudgetPlaner.Contracts.Api.Identity.TokenResponse tokenResponse)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            _logger.LogWarning("HttpContext is null, cannot update authentication state");
            return;
        }

        try
        {
            // Create new principal with updated tokens
            var newPrincipal = SignInService.GetPrincipal(tokenResponse);
            
            // Sign in with the new principal
            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, newPrincipal);
            
            // Update secure cookies
            httpContext.Response.Cookies.Append("accessToken", tokenResponse.AccessToken!, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(60)
            });
            
            httpContext.Response.Cookies.Append("refreshToken", tokenResponse.RefreshToken!, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(60)
            });
            
            _logger.LogInformation("Authentication state updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update authentication state");
        }
    }

    private async Task SignOutUserAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return;
        }

        try
        {
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            httpContext.Response.Cookies.Delete("accessToken");
            httpContext.Response.Cookies.Delete("refreshToken");
            
            _logger.LogInformation("User signed out due to authentication failure");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while signing out user");
        }
    }
} 