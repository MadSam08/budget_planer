using System.Text.Json;
using BudgetPlaner.Contracts.Api.Identity;
using Microsoft.AspNetCore.Identity.Data;

namespace BudgetPlaner.UI.ApiClients.Identity;

public class IdentityService(HttpClient client) : IIdentityService
{
    public async Task<bool> SignUpAsync(RegisterRequest request)
    {
        var response = await client.PostAsJsonAsync("budget-planer/account/register", request);
        return response.IsSuccessStatusCode;
    }
    
    public async Task<TokenResponse?> SignInAsync(LoginRequest request)
    {
        var response = await client.PostAsJsonAsync("budget-planer/account/login", request);
        
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<AccessTokenResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        if (result == null) return null;
        
        return new TokenResponse
        {
            AccessToken = result.AccessToken,
            RefreshToken = result.RefreshToken,
            ExpiresIn = result.ExpiresIn,
            TokenType = result.TokenType
        };
    }

    public async Task<TokenResponse?> RefreshToken(string? refreshToken)
    {
        var response = await client.PostAsJsonAsync("budget-planer/account/refresh", new RefreshRequest
        {
            RefreshToken = refreshToken!
        });
        
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<AccessTokenResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        if (result == null) return null;
        
        return new TokenResponse
        {
            AccessToken = result.AccessToken,
            RefreshToken = result.RefreshToken,
            ExpiresIn = result.ExpiresIn,
            TokenType = result.TokenType
        };
    }
}

// This matches the response from Identity API
public record AccessTokenResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public int ExpiresIn { get; init; }
    public string RefreshToken { get; init; } = string.Empty;
    public string TokenType { get; init; } = string.Empty;
}