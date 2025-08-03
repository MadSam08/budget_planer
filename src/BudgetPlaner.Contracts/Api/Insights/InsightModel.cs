namespace BudgetPlaner.Contracts.Api.Insights;

public record InsightModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public InsightType Type { get; set; }
    public InsightSeverity Severity { get; set; }
    public bool IsRead { get; set; }
    public bool ActionTaken { get; set; }
    public DateTime GeneratedDate { get; set; }
}

public enum InsightType
{
    SpendingPattern = 1,
    BudgetOverrun = 2,
    SavingsOpportunity = 3,
    IncomeFluctuation = 4,
    CategoryTrend = 5
}

public enum InsightSeverity
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
} 