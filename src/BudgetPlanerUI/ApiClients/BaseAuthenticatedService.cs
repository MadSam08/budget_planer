using System.Text.Json;

namespace BudgetPlaner.UI.ApiClients;

public abstract class BaseAuthenticatedService
{
    protected readonly HttpClient Client;
    protected readonly ILogger Logger;

    protected BaseAuthenticatedService(HttpClient client, ILogger logger)
    {
        Client = client;
        Logger = logger;
    }

    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    protected async Task<T?> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await Client.GetAsync(endpoint, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                Logger.LogWarning("GET request to {Endpoint} failed with status {StatusCode}", endpoint, response.StatusCode);
                return default;
            }
            
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<T>(content, JsonOptions);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error occurred during GET request to {Endpoint}", endpoint);
            return default;
        }
    }

    protected async Task<bool> PostAsync<T>(string endpoint, T data, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await Client.PostAsJsonAsync(endpoint, data, JsonOptions, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                Logger.LogWarning("POST request to {Endpoint} failed with status {StatusCode}", endpoint, response.StatusCode);
            }
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error occurred during POST request to {Endpoint}", endpoint);
            return false;
        }
    }

    protected async Task<bool> PutAsync<T>(string endpoint, T data, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await Client.PutAsJsonAsync(endpoint, data, JsonOptions, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                Logger.LogWarning("PUT request to {Endpoint} failed with status {StatusCode}", endpoint, response.StatusCode);
            }
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error occurred during PUT request to {Endpoint}", endpoint);
            return false;
        }
    }

    protected async Task<bool> PutAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await Client.PutAsync(endpoint, null, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                Logger.LogWarning("PUT request to {Endpoint} failed with status {StatusCode}", endpoint, response.StatusCode);
            }
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error occurred during PUT request to {Endpoint}", endpoint);
            return false;
        }
    }

    protected async Task<bool> DeleteAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await Client.DeleteAsync(endpoint, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                Logger.LogWarning("DELETE request to {Endpoint} failed with status {StatusCode}", endpoint, response.StatusCode);
            }
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error occurred during DELETE request to {Endpoint}", endpoint);
            return false;
        }
    }
} 