using BudgetPlaner.Domain;

namespace BudgetPlaner.Application.Services.Insights;

public interface IFinancialInsightService
{
    Task<IEnumerable<FinancialInsightEntity>> GenerateMonthlyInsightsAsync(string userId);
    Task<IEnumerable<FinancialInsightEntity>> GetUserInsightsAsync(string userId, bool unreadOnly = false);
    Task<FinancialInsightEntity> CreateInsightAsync(FinancialInsightEntity insight, string userId);
    Task<bool> MarkInsightAsReadAsync(int insightId, string userId);
    Task<bool> MarkInsightActionTakenAsync(int insightId, string userId);
    Task<IEnumerable<FinancialInsightEntity>> AnalyzeSpendingPatternsAsync(string userId);
    Task<IEnumerable<FinancialInsightEntity>> GenerateSavingsOpportunitiesAsync(string userId);
    Task<IEnumerable<FinancialInsightEntity>> AnalyzeBudgetPerformanceAsync(string userId);
    Task<FinancialInsightEntity?> GenerateLoanOptimizationSuggestionAsync(int loanId, string userId);
} 