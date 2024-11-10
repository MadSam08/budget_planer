namespace BudgetPlaner.Domain;

public record LoanEntity : BaseEntity
{
    public string? Name { get; set; }
    public int CurrencyId { get; set; }
    
    public string? BankName { get; set; }
    
    public decimal TotalAmount { get; set; }
    
    public decimal Principal { get; set; }
    
    public decimal Interest { get; set; }
    
    public decimal AnnualRate { get; set; }
    
    public CreditStatus CreditStatus { get; set; }
    
    /// <summary>
    /// DAE
    /// </summary>
    public decimal APR { get; set; }

    /// <summary>
    /// Represents the period of time for which a credit is taken in months.
    /// </summary>
    public int Period { get; set; }

    public virtual CurrencyEntity? Currency { get; set; }
    
    public virtual ICollection<LoanInterestRateEntity>? ScheduledRates { get; set; }
    
}