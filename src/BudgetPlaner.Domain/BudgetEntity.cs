namespace BudgetPlaner.Domain;

public record BudgetEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int CurrencyId { get; set; }
    public decimal TotalBudgetAmount { get; set; }
    public BudgetPeriodType PeriodType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public BudgetStatus Status { get; set; }
    
    // Navigation properties
    public virtual CurrencyEntity? Currency { get; set; }
    public virtual ICollection<BudgetCategoryEntity>? BudgetCategories { get; set; }
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