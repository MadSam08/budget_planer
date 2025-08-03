namespace BudgetPlaner.Domain;

public record BudgetEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int CurrencyId { get; set; }
    public decimal TotalBudgetAmount { get; set; }
    public BudgetPeriodType PeriodType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public BudgetStatus Status { get; set; }
    
    // Navigation properties
    public virtual CurrencyEntity? Currency { get; set; }
    public virtual ICollection<BudgetCategoryEntity>? BudgetCategories { get; set; }
}