namespace BudgetPlaner.Domain;

public record IncomeEntity : BaseEntity
{
    public int CurrencyId { get; set; }
    public int CategoryId { get; set; }
    public string? Description { get; set; }
    public decimal Value { get; set; }

    /// <summary>
    ///     Gets or sets the actual date of income when it was received, not the date on it was inserted in the system.
    /// </summary>
    public DateTime ActualDateOfIncome { get; set; }

    // Enhanced properties for better tracking
    public IncomeType Type { get; set; }
    public bool IsRecurring { get; set; }
    public string? Source { get; set; }
    public bool IsTaxable { get; set; }

    public virtual CurrencyEntity? Currency { get; set; }
    public virtual CategoryEntity? Category { get; set; }
}

public enum IncomeType
{
    Salary = 1,
    Freelance = 2,
    Investment = 3,
    Bonus = 4,
    Gift = 5,
    SideHustle = 6,
    Rental = 7,
    Other = 8
}