namespace BudgetPlaner.Models.Domain;

public record BaseEntity
{
    public int Id { get; set; }
    public required string UserId { get; set; }
    
    public DateTime CreateDate { get; set; }
    
    public DateTime UpdateDate { get; set; }
}