using BudgetPlaner.Contracts.Api.Category;
using BudgetPlaner.UI.Services;
using Microsoft.AspNetCore.Components.Authorization;

namespace BudgetPlaner.UI.ApiClients;

/// <summary>
/// Adapter service that implements ICategoryService using the SDK.
/// This service requires AuthenticationStateProvider to be passed from components.
/// </summary>
public class CategoryService : ICategoryService
{
    private readonly IBudgetPlanerSdkService _sdkService;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(IBudgetPlanerSdkService sdkService, ILogger<CategoryService> logger)
    {
        _sdkService = sdkService;
        _logger = logger;
    }

    public async Task<List<CategoryRequest>> GetCategoriesAsync(AuthenticationStateProvider authStateProvider, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting categories via SDK");
            var categories = await _sdkService.Categories.GetCategoriesAsync();
            
            var result = categories.Select(c => new CategoryRequest
            {
                Id = c.Id,
                Name = c.Name,
                CategoryTypes = c.CategoryTypes
            }).ToList();
            
            _logger.LogDebug("Retrieved {Count} categories", result.Count);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting categories");
            return new List<CategoryRequest>();
        }
    }

    public async Task<CategoryRequest?> GetCategoryAsync(AuthenticationStateProvider authStateProvider, string id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting category {Id} via SDK", id);
            var category = await _sdkService.Categories.GetCategoryAsync(id);
            
            if (category == null)
            {
                _logger.LogWarning("Category {Id} not found", id);
                return null;
            }

            // Convert CategoryModel to CategoryRequest for backward compatibility
            var result = new CategoryRequest
            {
                Id = category.Id,
                Name = category.Name,
                CategoryTypes = category.CategoryTypes
            };
            
            _logger.LogDebug("Retrieved category {Id}", id);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category {Id}", id);
            return null;
        }
    }

    public async Task<bool> CreateCategoryAsync(AuthenticationStateProvider authStateProvider, CategoryRequest category, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Creating category via SDK");
            await _sdkService.Categories.CreateCategoryAsync(category);
            
            _logger.LogDebug("Category creation succeeded");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            return false;
        }
    }

    public async Task<bool> UpdateCategoryAsync(AuthenticationStateProvider authStateProvider, string id, CategoryRequest category, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Updating category {Id} via SDK", id);
            await _sdkService.Categories.UpdateCategoryAsync(id, category);
            
            _logger.LogDebug("Category {Id} update succeeded", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category {Id}", id);
            return false;
        }
    }

    public async Task<bool> DeleteCategoryAsync(AuthenticationStateProvider authStateProvider, string id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Deleting category {Id} via SDK", id);
            await _sdkService.Categories.DeleteCategoryAsync(id);
            
            _logger.LogDebug("Category {Id} deletion succeeded", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category {Id}", id);
            return false;
        }
    }

    public async Task<bool> RestoreCategoryAsync(AuthenticationStateProvider authStateProvider, string id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Restoring category {Id} via SDK", id);
            await _sdkService.Categories.RestoreCategoryAsync(id);
            
            _logger.LogDebug("Category {Id} restoration succeeded", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring category {Id}", id);
            return false;
        }
    }
} 