using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using BudgetPlaner.Contracts.Claims;
using BudgetPlaner.UI.ApiClients.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;

namespace BudgetPlaner.UI.ApiClients;

public class AuthenticatedHttpMessageHandler : DelegatingHandler
{
    private readonly ITokenProvider _tokenProvider;
    private readonly ILogger<AuthenticatedHttpMessageHandler> _logger;
    private readonly SemaphoreSlim _refreshSemaphore = new(1, 1);

    public AuthenticatedHttpMessageHandler(
        ITokenProvider tokenProvider,
        ILogger<AuthenticatedHttpMessageHandler> logger)
    {
        _tokenProvider = tokenProvider;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Get access token and add to request
        var accessToken = await _tokenProvider.GetAccessTokenAsync();
        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        var response = await base.SendAsync(request, cancellationToken);

        // If we get 401 Unauthorized, try to refresh the token once
        if (response.StatusCode == HttpStatusCode.Unauthorized && !string.IsNullOrEmpty(accessToken))
        {
            var refreshed = await RefreshTokenWithSemaphoreAsync();
            if (refreshed)
            {
                // Get the new token and retry the request
                var newAccessToken = await _tokenProvider.GetAccessTokenAsync();
                if (!string.IsNullOrEmpty(newAccessToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newAccessToken);
                    response = await base.SendAsync(request, cancellationToken);
                }
            }
        }

        return response;
    }

    private async Task<bool> RefreshTokenWithSemaphoreAsync()
    {
        // Use semaphore to ensure only one refresh operation at a time
        await _refreshSemaphore.WaitAsync();
        
        try
        {
            return await _tokenProvider.RefreshTokenAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while refreshing access token");
            return false;
        }
        finally
        {
            _refreshSemaphore.Release();
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