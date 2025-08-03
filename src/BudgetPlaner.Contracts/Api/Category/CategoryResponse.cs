namespace BudgetPlaner.Contracts.Api.Category;

public class CategoryResponse
{
    public string Id { get; set; }
    public string Name { get; set; }
    public CategoryTypes CategoryTypes { get; set; }
}