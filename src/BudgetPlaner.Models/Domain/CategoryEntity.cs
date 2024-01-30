namespace BudgetPlaner.Models.Domain;

public record CategoryEntity : BaseEntity
{
    public required string Name { get; set; }
    public CategoryTypes CategoryTypes { get; set; }
    public bool IsDeleted { get; set; }
}