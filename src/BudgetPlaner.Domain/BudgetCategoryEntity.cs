namespace BudgetPlaner.Domain;

public record BudgetCategoryEntity : BaseEntity
{
    public int BudgetId { get; set; }
    public int CategoryId { get; set; }
    public decimal AllocatedAmount { get; set; }
    public decimal SpentAmount { get; set; }
    public decimal RemainingAmount => AllocatedAmount - SpentAmount;
    public bool IsOverBudget => SpentAmount > AllocatedAmount;
    
    // Navigation properties
    public virtual BudgetEntity? Budget { get; set; }
    public virtual CategoryEntity? Category { get; set; }
} 