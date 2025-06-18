using System.Net.Http.Headers;
using System.Net;
using BudgetPlaner.Contracts.Claims;
using BudgetPlaner.UI.ApiClients.Identity;
using BudgetPlaner.UI.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BudgetPlaner.UI.Infrastructure;

/// <summary>
/// Custom delegating handler for adding Authorization headers to outbound requests
/// This follows the Microsoft recommended pattern for authentication in Blazor Server
/// using IHttpContextAccessor instead of AuthenticationStateProvider to avoid DI scope issues
/// Includes automatic token refresh when access tokens are expired
/// </summary>
public class AuthTokenHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;
    private readonly ILogger<AuthTokenHandler> _logger;
    private readonly SemaphoreSlim _refreshSemaphore = new(1, 1);

    public AuthTokenHandler(
        IHttpContextAccessor httpContextAccessor, 
        IIdentityService identityService,
        ILogger<AuthTokenHandler> logger)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        // InnerHandler must be left as null when using DI
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated != true)
        {
            _logger.LogDebug("User not authenticated for request: {Method} {RequestUri}", 
                request.Method, request.RequestUri);
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }

        try
        {
            // Get access token and check if it needs refreshing
            var accessToken = await GetValidAccessTokenAsync(httpContext);
            
            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                _logger.LogDebug("Added Authorization header to request: {Method} {RequestUri}", 
                    request.Method, request.RequestUri);
            }
            else
            {
                _logger.LogWarning("No valid access token available for request: {Method} {RequestUri}", 
                    request.Method, request.RequestUri);
            }

            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            // If we get 401 Unauthorized, try to refresh the token and retry once
            if (response.StatusCode == HttpStatusCode.Unauthorized && !string.IsNullOrEmpty(accessToken))
            {
                _logger.LogDebug("Received 401 Unauthorized, attempting token refresh and retry for: {Method} {RequestUri}", 
                    request.Method, request.RequestUri);

                // Try to refresh the token
                var newAccessToken = await RefreshTokenAsync(httpContext);
                if (!string.IsNullOrEmpty(newAccessToken))
                {
                    // Create a new request with the refreshed token
                    var retryRequest = await CloneRequestAsync(request);
                    retryRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newAccessToken);
                    
                    _logger.LogDebug("Retrying request with refreshed token: {Method} {RequestUri}", 
                        retryRequest.Method, retryRequest.RequestUri);
                    
                    response.Dispose(); // Dispose the original unauthorized response
                    response = await base.SendAsync(retryRequest, cancellationToken).ConfigureAwait(false);
                }
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling authentication for request: {Method} {RequestUri}", 
                request.Method, request.RequestUri);
            // Continue with the request even if we can't add auth header
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task<string?> GetValidAccessTokenAsync(HttpContext httpContext)
    {
        // First try to get the access token from user claims
        var accessToken = httpContext.User.FindFirst(BudgetPlanerClaims.AccessToken)?.Value;
        
        // If not found in claims, try to get it from authentication properties
        if (string.IsNullOrEmpty(accessToken))
        {
            accessToken = await httpContext.GetTokenAsync("access_token");
        }

        if (string.IsNullOrEmpty(accessToken))
        {
            return null;
        }

        // Check if token is expired
        if (IsTokenExpired(httpContext))
        {
            _logger.LogDebug("Access token is expired, attempting refresh");
            return await RefreshTokenAsync(httpContext);
        }

        return accessToken;
    }

    private bool IsTokenExpired(HttpContext httpContext)
    {
        var expiresAtClaim = httpContext.User.FindFirst(BudgetPlanerClaims.ExpiresAt)?.Value;
        
        if (string.IsNullOrEmpty(expiresAtClaim))
        {
            // If we don't have expiration info, assume it might be expired and let the API decide
            return false;
        }

        if (DateTime.TryParse(expiresAtClaim, out var expiresAt))
        {
            // Add a 1-minute buffer to refresh before actual expiration
            return DateTime.UtcNow.AddMinutes(1) >= expiresAt;
        }

        return false;
    }

    private async Task<string?> RefreshTokenAsync(HttpContext httpContext)
    {
        // Use semaphore to prevent multiple concurrent refresh attempts
        await _refreshSemaphore.WaitAsync();
        try
        {
            // Get refresh token from claims or auth properties
            var refreshToken = httpContext.User.FindFirst(BudgetPlanerClaims.RefreshToken)?.Value;
            if (string.IsNullOrEmpty(refreshToken))
            {
                refreshToken = await httpContext.GetTokenAsync("refresh_token");
            }

            if (string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogWarning("No refresh token available");
                return null;
            }

            _logger.LogDebug("Refreshing access token");
            var tokenResponse = await _identityService.RefreshToken(refreshToken);
            
            if (tokenResponse == null)
            {
                _logger.LogWarning("Token refresh failed");
                return null;
            }

            // Create new principal with updated tokens
            var principal = SignInService.GetPrincipal(tokenResponse);
            
            // Create authentication properties with new tokens
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = httpContext.User.Identity?.AuthenticationType == CookieAuthenticationDefaults.AuthenticationScheme,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
            };
            
            // Store tokens in authentication properties as well
            authProperties.StoreTokens(new[]
            {
                new AuthenticationToken { Name = "access_token", Value = tokenResponse.AccessToken! },
                new AuthenticationToken { Name = "refresh_token", Value = tokenResponse.RefreshToken! },
                new AuthenticationToken { Name = "token_type", Value = tokenResponse.TokenType ?? "Bearer" },
                new AuthenticationToken { Name = "expires_at", Value = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn).ToString("o") }
            });
            
            // Sign in with new tokens
            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);
            
            _logger.LogDebug("Token refresh successful");
            return tokenResponse.AccessToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return null;
        }
        finally
        {
            _refreshSemaphore.Release();
        }
    }

    private static async Task<HttpRequestMessage> CloneRequestAsync(HttpRequestMessage originalRequest)
    {
        var clonedRequest = new HttpRequestMessage(originalRequest.Method, originalRequest.RequestUri);
        
        // Clone headers
        foreach (var header in originalRequest.Headers)
        {
            clonedRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        // Clone content if present
        if (originalRequest.Content != null)
        {
            var originalContent = await originalRequest.Content.ReadAsByteArrayAsync();
            clonedRequest.Content = new ByteArrayContent(originalContent);
            
            // Clone content headers
            foreach (var header in originalRequest.Content.Headers)
            {
                clonedRequest.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        return clonedRequest;
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