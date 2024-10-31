namespace BudgetPlaner.Models.Api;

public record SignInModel()
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
