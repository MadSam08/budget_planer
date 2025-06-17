using BudgetPlaner.Sdk.Constants;
using BudgetPlaner.Contracts.Api.Insights;
using Refit;

namespace BudgetPlaner.Sdk.Interfaces;

[Headers("Authorization: Bearer")]
public interface IInsightsApi
{
    [Get(ApiEndpoints.Insights.GetAll)]
    Task<IEnumerable<InsightModel>> GetInsightsAsync();

    [Get(ApiEndpoints.Insights.Get)]
    Task<InsightModel> GetInsightAsync(int id);

    [Post(ApiEndpoints.Insights.Create)]
    Task<IEnumerable<InsightModel>> GenerateInsightsAsync();

    // Insight Actions
    [Put(ApiEndpoints.Insights.MarkAsRead)]
    Task MarkInsightAsReadAsync(int id);

    [Put(ApiEndpoints.Insights.MarkActionTaken)]
    Task MarkInsightActionTakenAsync(int id);

    // Analysis Endpoints
    [Get(ApiEndpoints.Insights.SpendingPatterns)]
    Task<object> GetSpendingPatternsAsync();

    [Get(ApiEndpoints.Insights.SavingsOpportunities)]
    Task<object> GetSavingsOpportunitiesAsync();

    [Get(ApiEndpoints.Insights.BudgetPerformance)]
    Task<object> GetBudgetPerformanceAsync();
} 