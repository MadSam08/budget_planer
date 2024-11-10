namespace BudgetPlaner.Domain;

public record CurrencyEntity : BaseEntity
{
    public required string Name { get; set; }
    public string? Code { get; set; }
    public decimal NationalBankRate { get; set; }
    public bool IsDeleted { get; set; }
}