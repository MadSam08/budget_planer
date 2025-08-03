namespace BudgetPlaner.Contracts.Api.Budget;

public record BudgetRequest
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public required string CurrencyId { get; set; } 
    public decimal TotalBudgetAmount { get; set; }
    public BudgetPeriodType PeriodType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public BudgetStatus Status { get; set; }
}

public record BudgetCategoryRequest
{
    public int Id { get; set; }
    public int BudgetId { get; set; }
    public int CategoryId { get; set; }
    public decimal AllocatedAmount { get; set; }
    public decimal SpentAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public enum BudgetPeriodType
{
    Monthly = 1,
    Quarterly = 2,
    Yearly = 3
}

public enum BudgetStatus
{
    Active = 1,
    Inactive = 2,
    Completed = 3
} 