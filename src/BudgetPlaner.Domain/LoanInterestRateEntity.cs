namespace BudgetPlaner.Domain;

public record LoanInterestRateEntity : BaseEntity
{
    public int LoanId { get; set; }
    
    public InterestPayType InterestPayType { get; set; }
    
    public decimal PrincipalValue { get; set; }
    
    public decimal InterestValue { get; set; }
    
    public int Month { get; set; }
}