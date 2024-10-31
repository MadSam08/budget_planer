namespace BudgetPlaner.Models.Api;

public record CurrencyModel
{
    public string Id { get; set; }
    public required string Name { get; set; }
    public string? Code { get; set; }
    public decimal NationalBankRate { get; set; }
}