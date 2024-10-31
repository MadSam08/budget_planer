using BudgetPlaner.Models.Api;
using Microsoft.AspNetCore.Identity.Data;

namespace BudgetPlaner.UI.ApiClients.Identity;

public interface IIdentityService
{
    Task<bool> RegisterAsync(RegisterRequest request);

    Task<LoginResponse?> LoginAsync(LoginRequest request);
}