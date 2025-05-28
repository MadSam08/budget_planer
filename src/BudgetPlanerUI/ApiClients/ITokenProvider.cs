namespace BudgetPlaner.UI.ApiClients;

public interface ITokenProvider
{
    Task<string?> GetAccessTokenAsync();
    Task<string?> GetRefreshTokenAsync();
    Task<bool> RefreshTokenAsync();
} 