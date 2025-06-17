using BudgetPlaner.Contracts.Api.Category;
using BudgetPlaner.UI.Services;
using System.Text.Json;
using Microsoft.AspNetCore.Http.Features;
using System.Text;

namespace BudgetPlaner.UI.ApiClients;

public class CategoryService : ICategoryService
{
    private readonly HttpClient _httpClient;
    private readonly ITokenRefreshService _tokenRefreshService;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(HttpClient httpClient, ITokenRefreshService tokenRefreshService, ILogger<CategoryService> logger)
    {
        _httpClient = httpClient;
        _tokenRefreshService = tokenRefreshService;
        _logger = logger;
    }

    public async Task<List<CategoryRequest>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting categories via direct API call to /budget-planer/category");
            
            var response = await SendRequestWithRetryAsync(() => 
                _httpClient.GetAsync("/budget-planer/category", cancellationToken));
            
            _logger.LogDebug("Request URL: {RequestUri}", response.RequestMessage?.RequestUri);
            _logger.LogDebug("Response Status: {StatusCode}", response.StatusCode);
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var categories = JsonSerializer.Deserialize<CategoryRequest[]>(responseContent, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                }) ?? Array.Empty<CategoryRequest>();
                
                _logger.LogDebug("Retrieved {Count} categories", categories.Length);
                return categories.ToList();
            }
            
            _logger.LogWarning("Failed to get categories: {StatusCode}", response.StatusCode);
            return new List<CategoryRequest>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting categories");
            return new List<CategoryRequest>();
        }
    }

    public async Task<CategoryRequest?> GetCategoryAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting category {Id} via direct API", id);
            var response = await SendRequestWithRetryAsync(() => 
                _httpClient.GetAsync($"/budget-planer/category/{id}", cancellationToken));
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                var category = JsonSerializer.Deserialize<CategoryRequest>(json, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });
                
                _logger.LogDebug("Retrieved category {Id}", id);
                return category;
            }
            
            _logger.LogWarning("Failed to get category {Id}: {StatusCode}", id, response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category {Id}", id);
            return null;
        }
    }

    public async Task<bool> CreateCategoryAsync(CategoryRequest category, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Creating category via direct API");
            var response = await SendRequestWithRetryAsync(() => 
                _httpClient.PostAsJsonAsync("/budget-planer/category", category, cancellationToken));
            
            var success = response.IsSuccessStatusCode;
            _logger.LogDebug("Category creation {Result}", success ? "succeeded" : "failed");
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            return false;
        }
    }

    public async Task<bool> UpdateCategoryAsync(string id, CategoryRequest category, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Updating category {Id} via direct API", id);
            var response = await SendRequestWithRetryAsync(() => 
                _httpClient.PutAsJsonAsync($"/budget-planer/category/{id}", category, cancellationToken));
            
            var success = response.IsSuccessStatusCode;
            _logger.LogDebug("Category {Id} update {Result}", id, success ? "succeeded" : "failed");
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category {Id}", id);
            return false;
        }
    }

    public async Task<bool> DeleteCategoryAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Deleting category {Id} via direct API", id);
            var response = await SendRequestWithRetryAsync(() => 
                _httpClient.DeleteAsync($"/budget-planer/category/{id}", cancellationToken));
            
            var success = response.IsSuccessStatusCode;
            _logger.LogDebug("Category {Id} deletion {Result}", id, success ? "succeeded" : "failed");
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category {Id}", id);
            return false;
        }
    }

    public async Task<bool> RestoreCategoryAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Restoring category {Id} via direct API", id);
            var response = await SendRequestWithRetryAsync(() => 
                _httpClient.PatchAsync($"/budget-planer/category/{id}/restore", null, cancellationToken));
            
            var success = response.IsSuccessStatusCode;
            _logger.LogDebug("Category {Id} restoration {Result}", id, success ? "succeeded" : "failed");
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring category {Id}", id);
            return false;
        }
    }

    private async Task<HttpResponseMessage> SendRequestWithRetryAsync(Func<Task<HttpResponseMessage>> requestFunc)
    {
        var response = await requestFunc();
        
        // Handle 401 by refreshing token and retrying once
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            if (await _tokenRefreshService.RefreshTokenAsync())
            {
                response = await requestFunc();
            }
        }
        
        return response;
    }
} 