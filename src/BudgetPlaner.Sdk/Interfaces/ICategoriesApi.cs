using BudgetPlaner.Sdk.Constants;
using BudgetPlaner.Contracts.Api.Category;
using Refit;

namespace BudgetPlaner.Sdk.Interfaces;

[Headers("Authorization: Bearer","Content-Type: application/json; charset=UTF-8", "Accept: application/json")]
public interface ICategoriesApi
{
    [Get(ApiEndpoints.Categories.GetAll)]
    Task<IApiResponse<IEnumerable<CategoryResponse>>> GetCategoriesAsync();

    [Get(ApiEndpoints.Categories.Get)]
    Task<IApiResponse<CategoryResponse>?> GetCategoryAsync(string id);

    [Post(ApiEndpoints.Categories.Create)]
    Task<IApiResponse> CreateCategoryAsync([Body] CategoryRequest request);

    [Put(ApiEndpoints.Categories.Update)]
    Task<IApiResponse> UpdateCategoryAsync(string id, [Body] CategoryRequest request);

    [Delete(ApiEndpoints.Categories.Delete)]
    Task<IApiResponse> DeleteCategoryAsync(string id);

    [Patch(ApiEndpoints.Categories.Restore)]
    Task<IApiResponse> RestoreCategoryAsync(string id);
} 