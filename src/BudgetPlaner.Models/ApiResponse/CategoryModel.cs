using BudgetPlaner.Models.Domain;

namespace BudgetPlaner.Models.ApiResponse;

public record CategoryModel
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public CategoryTypes CategoryTypes { get; set; }
}