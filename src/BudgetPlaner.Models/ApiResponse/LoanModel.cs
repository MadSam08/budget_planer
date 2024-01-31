using BudgetPlaner.Models.Domain;

namespace BudgetPlaner.Models.ApiResponse;

public record LoanModel
{
    public required string Id { get; set; }
    public string? Name { get; set; }
    public int CurrencyId { get; set; }
    
    public string? BankName { get; set; }
    
    public decimal TotalValue { get; set; }
    
    public decimal Principal { get; set; }
    
    public decimal Interest { get; set; }
    
    public decimal AnnualRate { get; set; }
    
    public string? CurrencyName { get; set; }
    
    public CreditStatus CreditStatus { get; set; }
    
    /// <summary>
    /// DAE
    /// </summary>
    public decimal APR { get; set; }
    
    public int Period { get; set; }
}