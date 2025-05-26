namespace BudgetPlaner.Domain;

public record LoanPaymentEntity : BaseEntity
{
    public int LoanId { get; set; }
    public decimal Amount { get; set; }
    public decimal PrincipalAmount { get; set; }
    public decimal InterestAmount { get; set; }
    public DateTime PaymentDate { get; set; }
    public DateTime DueDate { get; set; }
    public PaymentStatus Status { get; set; }
    public PaymentType Type { get; set; }
    public string? Notes { get; set; }
    
    // Navigation properties
    public virtual LoanEntity? Loan { get; set; }
}

public enum PaymentStatus
{
    Scheduled = 1,
    Paid = 2,
    Late = 3,
    Missed = 4
}

public enum PaymentType
{
    Regular = 1,
    ExtraPayment = 2,
    EarlyPayment = 3
} 