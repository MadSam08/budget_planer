using BudgetPlaner.Domain;

namespace BudgetPlaner.Application.Services.Budget;

public interface IBudgetService
{
    Task<BudgetEntity> CreateBudgetAsync(BudgetEntity budget, string userId);
    Task<BudgetEntity?> GetBudgetByIdAsync(int budgetId, string userId);
    Task<IEnumerable<BudgetEntity>> GetUserBudgetsAsync(string userId);
    Task<BudgetEntity> UpdateBudgetAsync(BudgetEntity budget, string userId);
    Task<bool> DeleteBudgetAsync(int budgetId, string userId);
    Task<BudgetCategoryEntity> AddCategoryToBudgetAsync(int budgetId, int categoryId, decimal allocatedAmount, string userId);
    Task<BudgetCategoryEntity> UpdateBudgetCategoryAsync(int budgetCategoryId, decimal allocatedAmount, string userId);
    Task<bool> RemoveCategoryFromBudgetAsync(int budgetCategoryId, string userId);
    Task<decimal> GetBudgetUtilizationAsync(int budgetId, string userId);
    Task<IEnumerable<BudgetCategoryEntity>> GetOverBudgetCategoriesAsync(int budgetId, string userId);
} 