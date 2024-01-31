using BudgetPlaner.Models.Domain;

namespace BudgetPlaner.Models.ApiResponse;

public record LoanInterestRateModel
{
    public int CreditId { get; set; }
    
    public InterestPayType InterestPayType { get; set; }
    
    public decimal PrincipalValue { get; set; }
    
    public decimal InterestValue { get; set; }
}