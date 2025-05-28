using BudgetPlaner.Contracts.Api.Category;
using Microsoft.Extensions.Logging;

namespace BudgetPlaner.UI.ApiClients;

public class CategoryService : BaseAuthenticatedService, ICategoryService
{
    private const string BasePath = "budget-planer/category";

    public CategoryService(HttpClient client, ILogger<CategoryService> logger) 
        : base(client, logger)
    {
    }

    public async Task<List<CategoryRequest>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var result = await GetAsync<List<CategoryRequest>>(BasePath, cancellationToken);
        return result ?? new List<CategoryRequest>();
    }

    public async Task<CategoryRequest?> GetCategoryAsync(string id, CancellationToken cancellationToken = default)
    {
        return await GetAsync<CategoryRequest>($"{BasePath}/{id}", cancellationToken);
    }

    public async Task<bool> CreateCategoryAsync(CategoryRequest category, CancellationToken cancellationToken = default)
    {
        return await PostAsync(BasePath, category, cancellationToken);
    }

    public async Task<bool> UpdateCategoryAsync(string id, CategoryRequest category, CancellationToken cancellationToken = default)
    {
        return await PutAsync($"{BasePath}/{id}", category, cancellationToken);
    }

    public async Task<bool> DeleteCategoryAsync(string id, CancellationToken cancellationToken = default)
    {
        return await DeleteAsync($"{BasePath}/{id}", cancellationToken);
    }

    public async Task<bool> RestoreCategoryAsync(string id, CancellationToken cancellationToken = default)
    {
        return await PutAsync($"{BasePath}/restore/{id}", cancellationToken);
    }
} 