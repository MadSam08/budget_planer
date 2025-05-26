namespace BudgetPlaner.Domain;

public record SavingsGoalEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CurrencyId { get; set; }
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public DateTime TargetDate { get; set; }
    public SavingsGoalStatus Status { get; set; }
    public SavingsGoalPriority Priority { get; set; }
    
    // Calculated properties
    public decimal RemainingAmount => TargetAmount - CurrentAmount;
    public decimal ProgressPercentage => TargetAmount > 0 ? (CurrentAmount / TargetAmount) * 100 : 0;
    public int DaysRemaining => (TargetDate - DateTime.UtcNow).Days;
    public decimal RequiredMonthlySaving => DaysRemaining > 0 ? RemainingAmount / (DaysRemaining / 30.0m) : 0;
    
    // Navigation properties
    public virtual CurrencyEntity? Currency { get; set; }
    public virtual ICollection<SavingsContributionEntity>? Contributions { get; set; }
}

public enum SavingsGoalStatus
{
    Active = 1,
    Completed = 2,
    Paused = 3,
    Cancelled = 4
}

public enum SavingsGoalPriority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
} 