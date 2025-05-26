namespace BudgetPlaner.Domain;

public record FinancialInsightEntity : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public InsightType Type { get; set; }
    public InsightPriority Priority { get; set; }
    public decimal? PotentialSavings { get; set; }
    public int? CategoryId { get; set; }
    public bool IsRead { get; set; }
    public bool IsActionTaken { get; set; }
    public DateTime ValidUntil { get; set; }
    
    // Navigation properties
    public virtual CategoryEntity? Category { get; set; }
}

public enum InsightType
{
    SpendingAlert = 1,
    SavingsOpportunity = 2,
    BudgetRecommendation = 3,
    LoanOptimization = 4,
    CategoryAnalysis = 5,
    MonthlyReport = 6
}

public enum InsightPriority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
} 