using System.Net.Http.Headers;
using BudgetPlaner.Contracts.Claims;
using Microsoft.AspNetCore.Authentication;

namespace BudgetPlaner.UI.Infrastructure;

/// <summary>
/// Custom delegating handler for adding Authorization headers to outbound requests
/// This follows the Microsoft recommended pattern for authentication in Blazor Server
/// using IHttpContextAccessor instead of AuthenticationStateProvider to avoid DI scope issues
/// </summary>
public class AuthTokenHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthTokenHandler> _logger;

    public AuthTokenHandler(IHttpContextAccessor httpContextAccessor, ILogger<AuthTokenHandler> logger)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        // InnerHandler must be left as null when using DI
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                // First try to get the access token from user claims
                var accessToken = httpContext.User.FindFirst(BudgetPlanerClaims.AccessToken)?.Value;
                
                // If not found in claims, try to get it from authentication properties
                if (string.IsNullOrEmpty(accessToken))
                {
                    accessToken = await httpContext.GetTokenAsync("access_token");
                }

                if (!string.IsNullOrEmpty(accessToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    _logger.LogDebug("Added Authorization header to request: {Method} {RequestUri}", 
                        request.Method, request.RequestUri);
                }
                else
                {
                    _logger.LogWarning("Access token not found for authenticated user in request: {Method} {RequestUri}", 
                        request.Method, request.RequestUri);
                }
            }
            else
            {
                _logger.LogDebug("User not authenticated for request: {Method} {RequestUri}", 
                    request.Method, request.RequestUri);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding authentication header to request: {Method} {RequestUri}", 
                request.Method, request.RequestUri);
            // Continue with the request even if we can't add auth header
            // This allows unauthenticated endpoints to work
        }

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
} 