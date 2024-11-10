using System.Text.Json;
using BudgetPlaner.Models.Api;
using Microsoft.AspNetCore.Identity.Data;

namespace BudgetPlaner.UI.ApiClients.Identity;

public class IdentityService(HttpClient client) : IIdentityService
{
    public async Task<bool> SignUpAsync(RegisterRequest request)
    {
        var response = await client.PostAsJsonAsync("budget-planer/account/register", request);
        return response.IsSuccessStatusCode;
    }
    
    public async Task<LoginResponse?> SignInAsync(LoginRequest request)
    {
        var response = await client.PostAsJsonAsync("budget-planer/account/login", request);
        
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<LoginResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return result;
    }

    public Task<TokenResponse> RefreshToken(string? refreshToken)
    {
        
    }
}