using BudgetPlaner.Contracts.Claims;
using Microsoft.AspNetCore.Authentication;

namespace BudgetPlaner.UI.Infrastructure;

public class AuthTokenHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthTokenHandler> _logger;
    private readonly SemaphoreSlim _refreshSemaphore = new(1, 1);
    private static volatile bool _refreshInProgress = false;

    public AuthTokenHandler(
        IHttpContextAccessor httpContextAccessor, 
        ILogger<AuthTokenHandler> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        
        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            // Get current access token
            var accessToken = await GetValidAccessTokenAsync(httpContext);
            
            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            }
        }

        var response = await base.SendAsync(request, cancellationToken);
        
        // If we get 401 Unauthorized, the token needs refresh
        // But we can't refresh it here due to headers being read-only
        // The client will need to handle this by calling the refresh endpoint
        
        return response;
    }

    private async Task<string?> GetValidAccessTokenAsync(HttpContext httpContext)
    {
        // Try to get token from claims first
        var accessTokenFromClaims = httpContext.User.FindFirst(BudgetPlanerClaims.AccessToken)?.Value;
        
        // Fallback to GetTokenAsync
        string? accessTokenFromAuth = null;
        try
        {
            accessTokenFromAuth = await httpContext.GetTokenAsync("access_token");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "AuthTokenHandler: Error getting token from GetTokenAsync");
        }

        // Use claims token first, then fallback to GetTokenAsync
        var accessToken = !string.IsNullOrEmpty(accessTokenFromClaims) ? accessTokenFromClaims : accessTokenFromAuth;
        
        if (string.IsNullOrEmpty(accessToken))
        {
            _logger.LogWarning("AuthTokenHandler: No access token found");
            return null;
        }

        // Check if token is expired or about to expire (within 2 minutes)
        if (IsTokenExpiredOrExpiring(accessToken))
        {
            _logger.LogDebug("AuthTokenHandler: Access token is expired or expiring");
            // Don't try to refresh here - return null to force 401, which the client can handle
            return null;
        }

        return accessToken;
    }

    private bool IsTokenExpiredOrExpiring(string accessToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return true;
            }

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return true;
            }

            // Get the expiry time from claims instead of parsing JWT
            var expiresAtClaim = httpContext.User.FindFirst(BudgetPlanerClaims.ExpiresAt)?.Value;
            
            if (string.IsNullOrEmpty(expiresAtClaim))
            {
                _logger.LogWarning("AuthTokenHandler: No ExpiresAt claim found, treating token as expired");
                return true;
            }

            if (!DateTime.TryParse(expiresAtClaim, out var expiryTime))
            {
                _logger.LogWarning("AuthTokenHandler: Could not parse ExpiresAt claim value");
                return true;
            }

            // Ensure expiry time is in UTC
            if (expiryTime.Kind != DateTimeKind.Utc)
            {
                expiryTime = expiryTime.ToUniversalTime();
            }
            
            // Check if token expires within the next 2 minutes
            var bufferTime = DateTime.UtcNow.AddMinutes(2);
            var isExpiring = expiryTime <= bufferTime;
            
            return isExpiring;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AuthTokenHandler: Error checking token expiry");
            return true; // Assume expired if we can't check it
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _refreshSemaphore?.Dispose();
        }
        base.Dispose(disposing);
    }
} 