namespace BudgetPlaner.Contracts.Api.Loan;

public record LoanInterestRateModel
{
    public int CreditId { get; set; }
    
    public int InterestPayType { get; set; }
    
    public decimal PrincipalValue { get; set; }
    
    public decimal InterestValue { get; set; }
}