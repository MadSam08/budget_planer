using BudgetPlaner.Contracts.Api.Category;

namespace BudgetPlaner.UI.ApiClients;

public interface ICategoryService
{
    Task<List<CategoryRequest>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<CategoryRequest?> GetCategoryAsync(string id, CancellationToken cancellationToken = default);
    Task<bool> CreateCategoryAsync(CategoryRequest category, CancellationToken cancellationToken = default);
    Task<bool> UpdateCategoryAsync(string id, CategoryRequest category, CancellationToken cancellationToken = default);
    Task<bool> DeleteCategoryAsync(string id, CancellationToken cancellationToken = default);
    Task<bool> RestoreCategoryAsync(string id, CancellationToken cancellationToken = default);
} 