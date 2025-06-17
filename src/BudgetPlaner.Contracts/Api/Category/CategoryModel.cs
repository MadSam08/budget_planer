namespace BudgetPlaner.Contracts.Api.Category;

public record CategoryModel
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public CategoryTypes CategoryTypes { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
} 