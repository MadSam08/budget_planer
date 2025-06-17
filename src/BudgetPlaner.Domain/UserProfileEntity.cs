namespace BudgetPlaner.Domain;

public record UserProfileEntity : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public string ProviderUserId { get; set; } = string.Empty;
}
