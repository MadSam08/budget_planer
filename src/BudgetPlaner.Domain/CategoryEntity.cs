namespace BudgetPlaner.Domain;

public record CategoryEntity : BaseEntity
{
    public required string Name { get; set; }
    public int CategoryTypes { get; set; }
    public bool IsDeleted { get; set; }
}