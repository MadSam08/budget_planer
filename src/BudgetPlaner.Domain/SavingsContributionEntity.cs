namespace BudgetPlaner.Domain;

public record SavingsContributionEntity : BaseEntity
{
    public int SavingsGoalId { get; set; }
    public decimal Amount { get; set; }
    public DateTime ContributionDate { get; set; }
    public string? Notes { get; set; }
    public ContributionType Type { get; set; }
    
    // Navigation properties
    public virtual SavingsGoalEntity? SavingsGoal { get; set; }
}

public enum ContributionType
{
    Manual = 1,
    Automatic = 2,
    Bonus = 3,
    Interest = 4
} 