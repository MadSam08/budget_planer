namespace BudgetPlaner.Contracts.Api.Identity;

public record RefreshRequest
{
    public required string RefreshToken { get; set; }
} 