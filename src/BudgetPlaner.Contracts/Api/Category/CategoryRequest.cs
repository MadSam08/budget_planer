namespace BudgetPlaner.Contracts.Api.Category;

public record CategoryRequest
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public CategoryTypes CategoryTypes { get; set; }
}