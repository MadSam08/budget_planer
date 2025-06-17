using System.Text.Json;

namespace BudgetPlaner.UI.Services;

public interface ITokenRefreshService
{
    Task<bool> RefreshTokenAsync();
}

public class TokenRefreshService : ITokenRefreshService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TokenRefreshService> _logger;
    private readonly SemaphoreSlim _refreshSemaphore = new(1, 1);

    public TokenRefreshService(HttpClient httpClient, ILogger<TokenRefreshService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> RefreshTokenAsync()
    {
        // Use semaphore to prevent multiple concurrent refresh attempts
        await _refreshSemaphore.WaitAsync();
        try
        {
            _logger.LogDebug("Attempting token refresh via /api/auth/refresh");
            _logger.LogDebug("HttpClient BaseAddress: {BaseAddress}", _httpClient.BaseAddress);
            
            var response = await _httpClient.PostAsync("/api/auth/refresh", null);
            
            _logger.LogDebug("Token refresh response: {StatusCode}", response.StatusCode);
            _logger.LogDebug("Response headers: {Headers}", string.Join(", ", response.Headers.Select(h => $"{h.Key}={string.Join(",", h.Value)}")));
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogDebug("Token refresh successful");
                return true;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Token refresh failed: {StatusCode} - {Error}", 
                    response.StatusCode, errorContent);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return false;
        }
        finally
        {
            _refreshSemaphore.Release();
        }
    }
} 