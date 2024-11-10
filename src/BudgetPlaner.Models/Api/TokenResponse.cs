namespace BudgetPlaner.Models.Api;

public record TokenResponse
{
    public string? AccessToken { get; init; }
    public string? RefreshToken { get; init; }
}