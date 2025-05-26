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
    public LoanStatus Status { get; set; }
    
    /// <summary>
    /// DAE - Annual Percentage Rate
    /// </summary>
    public decimal APR { get; set; }

    /// <summary>
    /// Represents the period of time for which a credit is taken in months.
    /// </summary>
    public int Period { get; set; }
    
    // Enhanced properties
    public LoanType Type { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? MaturityDate { get; set; }
    public decimal MonthlyPayment { get; set; }
    public decimal RemainingBalance { get; set; }
    public int PaymentsMade { get; set; }
    public int RemainingPayments => Period - PaymentsMade;

    public virtual CurrencyEntity? Currency { get; set; }
    public virtual ICollection<LoanInterestRateEntity>? ScheduledRates { get; set; }
    public virtual ICollection<LoanPaymentEntity>? Payments { get; set; }
}

public enum LoanStatus
{
    Active = 1,
    PaidOff = 2,
    Defaulted = 3,
    Refinanced = 4
}

public enum LoanType
{
    Personal = 1,
    Mortgage = 2,
    Auto = 3,
    Student = 4,
    CreditCard = 5,
    Business = 6
}