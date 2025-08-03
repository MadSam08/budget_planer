namespace BudgetPlaner.Contracts.Api.Loan;

public record SpendingRequest
{
    public required string Id { get; set; }
    public string CurrencyId { get; set; }
    public string CategoryId { get; set; }
    
    public string BudgetId { get; set; }

    public string? Description { get; set; }
    public decimal Value { get; set; }

    /// <summary>
    ///     Gets or sets the actual date of income when it was received, not the date on it was inserted in the system.
    /// </summary>
    public DateTime ActualDateOfSpending { get; set; }
    
    public string? CurrencyName { get; set; }
    public string? CategoryName { get; set; }
}