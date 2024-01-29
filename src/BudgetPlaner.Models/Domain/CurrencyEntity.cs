namespace BudgetPlaner.Models.Domain;

public record CurrencyEntity : BaseEntity
{
    public required string Name { get; set; }
    public string? Code { get; set; }
    public decimal NationalBankRate { get; set; }
}