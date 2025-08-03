using BudgetPlaner.Sdk.Constants;
using BudgetPlaner.Contracts.Api.Budget;
using Refit;

namespace BudgetPlaner.Sdk.Interfaces;

[Headers("Authorization: Bearer","Content-Type: application/json; charset=UTF-8", "Accept: application/json")]
public interface IBudgetsApi
{
    [Get(ApiEndpoints.Budgets.GetAll)]
    Task<IApiResponse<IEnumerable<BudgetResponse>>> GetBudgetsAsync();

    [Get(ApiEndpoints.Budgets.Get)]
    Task<IApiResponse<BudgetRequest>> GetBudgetAsync(int id);

    [Post(ApiEndpoints.Budgets.Create)]
    Task<IApiResponse> CreateBudgetAsync([Body] BudgetRequest request);

    [Put(ApiEndpoints.Budgets.Update)]
    Task<IApiResponse> UpdateBudgetAsync(int id, [Body] BudgetRequest request);

    [Delete(ApiEndpoints.Budgets.Delete)]
    Task<IApiResponse> DeleteBudgetAsync(string id);

    // Budget Categories - Nested Resources
    [Get(ApiEndpoints.Budgets.GetCategories)]
    Task<IEnumerable<BudgetCategoryRequest>> GetBudgetCategoriesAsync(int budgetId);

    [Post(ApiEndpoints.Budgets.AddCategory)]
    Task<BudgetCategoryRequest> AddCategoryToBudgetAsync(int budgetId, [Body] BudgetCategoryRequest request);

    [Put(ApiEndpoints.Budgets.UpdateCategory)]
    Task<BudgetCategoryRequest> UpdateBudgetCategoryAsync(int budgetId, int categoryId, [Body] BudgetCategoryRequest request);

    [Delete(ApiEndpoints.Budgets.RemoveCategory)]
    Task RemoveCategoryFromBudgetAsync(int budgetId, int categoryId);

    // Budget Analysis - Sub-resources
    [Get(ApiEndpoints.Budgets.GetUtilization)]
    Task<object> GetBudgetUtilizationAsync(int id);

    [Get(ApiEndpoints.Budgets.GetOverBudget)]
    Task<IEnumerable<BudgetCategoryRequest>> GetOverBudgetCategoriesAsync(int id);
} 