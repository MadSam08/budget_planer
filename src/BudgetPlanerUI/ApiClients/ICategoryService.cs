using BudgetPlaner.Contracts.Api.Category;
using Microsoft.AspNetCore.Components.Authorization;

namespace BudgetPlaner.UI.ApiClients;

public interface ICategoryService
{
    Task<List<CategoryRequest>> GetCategoriesAsync(AuthenticationStateProvider authStateProvider,
        CancellationToken cancellationToken = default);

    Task<CategoryRequest?> GetCategoryAsync(AuthenticationStateProvider authStateProvider, string id,
        CancellationToken cancellationToken = default);

    Task<bool> CreateCategoryAsync(AuthenticationStateProvider authStateProvider, CategoryRequest category,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateCategoryAsync(AuthenticationStateProvider authStateProvider, string id, CategoryRequest category,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteCategoryAsync(AuthenticationStateProvider authStateProvider, string id,
        CancellationToken cancellationToken = default);

    Task<bool> RestoreCategoryAsync(AuthenticationStateProvider authStateProvider, string id,
        CancellationToken cancellationToken = default);
} 