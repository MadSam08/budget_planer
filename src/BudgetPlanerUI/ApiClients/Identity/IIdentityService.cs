using BudgetPlaner.Contracts.Api;
using BudgetPlaner.Contracts.Api.Identity;
using Microsoft.AspNetCore.Identity.Data;

namespace BudgetPlaner.UI.ApiClients.Identity;

public interface IIdentityService
{
    Task<bool> SignUpAsync(RegisterRequest request);

    Task<TokenResponse?> SignInAsync(LoginRequest request);
    
    Task<TokenResponse?> RefreshToken(string? refreshToken);
}