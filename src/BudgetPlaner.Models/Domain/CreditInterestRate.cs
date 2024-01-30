namespace BudgetPlaner.Models.Domain;

public record CreditInterestRate : BaseEntity
{
    public int CreditId { get; set; }
    
    public InterestRateType InterestRateType { get; set; }
    
    public decimal PrincipalValue { get; set; }
    
    public decimal InterestValue { get; set; }
}