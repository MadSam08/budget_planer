using BudgetPlaner.Domain;

namespace BudgetPlaner.Application.Services.Savings;

public interface ISavingsGoalService
{
    Task<SavingsGoalEntity> CreateSavingsGoalAsync(SavingsGoalEntity savingsGoal, string userId);
    Task<SavingsGoalEntity?> GetSavingsGoalByIdAsync(int goalId, string userId);
    Task<IEnumerable<SavingsGoalEntity>> GetUserSavingsGoalsAsync(string userId);
    Task<SavingsGoalEntity> UpdateSavingsGoalAsync(SavingsGoalEntity savingsGoal, string userId);
    Task<bool> DeleteSavingsGoalAsync(int goalId, string userId);
    Task<SavingsContributionEntity> AddContributionAsync(int goalId, decimal amount, ContributionType type, string? notes, string userId);
    Task<IEnumerable<SavingsContributionEntity>> GetGoalContributionsAsync(int goalId, string userId);
    Task<decimal> CalculateRequiredMonthlySavingAsync(int goalId, string userId);
    Task<IEnumerable<SavingsGoalEntity>> GetGoalsNearingDeadlineAsync(string userId, int daysThreshold = 30);
    Task<bool> MarkGoalAsCompletedAsync(int goalId, string userId);
} 