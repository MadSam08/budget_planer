using BudgetPlaner.Sdk.Constants;
using BudgetPlaner.Contracts.Api.Category;
using Refit;

namespace BudgetPlaner.Sdk.Interfaces;

[Headers("Authorization: Bearer")]
public interface ICategoriesApi
{
    [Get(ApiEndpoints.Categories.GetAll)]
    Task<IEnumerable<CategoryModel>> GetCategoriesAsync();

    [Get(ApiEndpoints.Categories.Get)]
    Task<CategoryModel> GetCategoryAsync(string id);

    [Post(ApiEndpoints.Categories.Create)]
    Task<CategoryModel> CreateCategoryAsync([Body] CategoryRequest request);

    [Put(ApiEndpoints.Categories.Update)]
    Task<CategoryModel> UpdateCategoryAsync(string id, [Body] CategoryRequest request);

    [Delete(ApiEndpoints.Categories.Delete)]
    Task DeleteCategoryAsync(string id);

    [Patch(ApiEndpoints.Categories.Restore)]
    Task RestoreCategoryAsync(string id);
} 