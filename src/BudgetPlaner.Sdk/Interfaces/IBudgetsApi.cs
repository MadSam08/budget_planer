using BudgetPlaner.Sdk.Constants;
using BudgetPlaner.Contracts.Api.Budget;
using Refit;

namespace BudgetPlaner.Sdk.Interfaces;

[Headers("Authorization: Bearer")]
public interface IBudgetsApi
{
    [Get(ApiEndpoints.Budgets.GetAll)]
    Task<IEnumerable<BudgetModel>> GetBudgetsAsync();

    [Get(ApiEndpoints.Budgets.Get)]
    Task<BudgetModel> GetBudgetAsync(int id);

    [Post(ApiEndpoints.Budgets.Create)]
    Task<BudgetModel> CreateBudgetAsync([Body] BudgetModel request);

    [Put(ApiEndpoints.Budgets.Update)]
    Task<BudgetModel> UpdateBudgetAsync(int id, [Body] BudgetModel request);

    [Delete(ApiEndpoints.Budgets.Delete)]
    Task DeleteBudgetAsync(int id);

    // Budget Categories - Nested Resources
    [Get(ApiEndpoints.Budgets.GetCategories)]
    Task<IEnumerable<BudgetCategoryModel>> GetBudgetCategoriesAsync(int budgetId);

    [Post(ApiEndpoints.Budgets.AddCategory)]
    Task<BudgetCategoryModel> AddCategoryToBudgetAsync(int budgetId, [Body] BudgetCategoryModel request);

    [Put(ApiEndpoints.Budgets.UpdateCategory)]
    Task<BudgetCategoryModel> UpdateBudgetCategoryAsync(int budgetId, int categoryId, [Body] BudgetCategoryModel request);

    [Delete(ApiEndpoints.Budgets.RemoveCategory)]
    Task RemoveCategoryFromBudgetAsync(int budgetId, int categoryId);

    // Budget Analysis - Sub-resources
    [Get(ApiEndpoints.Budgets.GetUtilization)]
    Task<object> GetBudgetUtilizationAsync(int id);

    [Get(ApiEndpoints.Budgets.GetOverBudget)]
    Task<IEnumerable<BudgetCategoryModel>> GetOverBudgetCategoriesAsync(int id);
} 