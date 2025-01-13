namespace BudgetPlaner.Domain;

public record LoanInterestRateEntity : BaseEntity
{
    public int LoanId { get; set; }
    
    public int InterestPayType { get; set; }
    
    public decimal PrincipalValue { get; set; }
    
    public decimal InterestValue { get; set; }
    
    public int Month { get; set; }
}