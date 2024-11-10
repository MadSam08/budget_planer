using BudgetPlaner.Domain;

namespace BudgetPlaner.Models.Api;

public record LoanInterestRateModel
{
    public int CreditId { get; set; }
    
    public InterestPayType InterestPayType { get; set; }
    
    public decimal PrincipalValue { get; set; }
    
    public decimal InterestValue { get; set; }
}