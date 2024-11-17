using BudgetPlaner.Models.Api;
using Microsoft.AspNetCore.Identity.Data;

namespace BudgetPlaner.UI.ApiClients.Identity;

public interface IIdentityService
{
    Task<bool> SignUpAsync(RegisterRequest request);

    Task<LoginResponse?> SignInAsync(LoginRequest request);
    
    Task<TokenResponse?> RefreshToken(string? refreshToken);
}