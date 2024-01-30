namespace BudgetPlaner.Models.Domain;

public record CreditEntity : BaseEntity
{
    public int CurrencyId { get; set; }
    
    public string? BankName { get; set; }
    
    public decimal TotalValue { get; set; }
    
    public decimal Principal { get; set; }
    
    public decimal Interest { get; set; }
    
    public decimal AnnualRate { get; set; }
    
    /// <summary>
    /// DAE
    /// </summary>
    public decimal APR { get; set; }
    
    public int Period { get; set; }
    
    public virtual CurrencyEntity? Currency { get; set; }
    
    public virtual ICollection<CreditInterestRate>? InterestRates { get; set; }
    
}