namespace BudgetPlaner.Domain;

public record SpendingEntity : BaseEntity
{
    public int CurrencyId { get; set; }
    public int CategoryId { get; set; }
    public string? Description { get; set; }
    public decimal Value { get; set; }
    
    /// <summary>
    /// Gets or sets the actual date of spending when it was received, not the date on it was inserted in the system.
    /// </summary>
    public DateTime ActualDateOfSpending { get; set; }
    
    // Enhanced properties for better tracking
    public string? Merchant { get; set; }
    public string? Location { get; set; }
    public SpendingType Type { get; set; }
    public bool IsRecurring { get; set; }
    public string? Tags { get; set; } // JSON array of tags
    
    public virtual CurrencyEntity? Currency { get; set; }
    public virtual CategoryEntity? Category { get; set; }
}

public enum SpendingType
{
    Cash = 1,
    Card = 2,
    BankTransfer = 3,
    DigitalWallet = 4,
    Check = 5
}
