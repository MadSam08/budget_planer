namespace BudgetPlaner.Contracts.Api.Identity;

public record TokenResponse
{
    public string? AccessToken { get; init; }
    public string? RefreshToken { get; init; }
    public int ExpiresIn { get; init; }
    public string? TokenType { get; init; }
}