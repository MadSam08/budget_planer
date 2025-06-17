using System.Text.Json;
using BudgetPlaner.Contracts.Api.Identity;
using BudgetPlaner.Contracts.UI.Identity;
using Microsoft.AspNetCore.Identity.Data;
using RefreshRequest = Microsoft.AspNetCore.Identity.Data.RefreshRequest;

namespace BudgetPlaner.UI.ApiClients.Identity;

public class IdentityService : IIdentityService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<IdentityService> _logger;

    // API endpoint constants following the same pattern as the SDK
    private const string BaseAuthPath = "api/auth";
    private const string SignUpEndpoint = $"{BaseAuthPath}/register";
    private const string SignInEndpoint = $"{BaseAuthPath}/login";
    private const string RefreshTokenEndpoint = $"{BaseAuthPath}/refresh-token";

    public IdentityService(
        HttpClient httpClient,
        ILogger<IdentityService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> SignUpAsync(RegisterRequest request)
    {
        try
        {
            // Convert RegisterRequest to SignUpModel
            var signUpModel = new SignUpModel
            {
                Email = request.Email,
                Password = request.Password,
                ConfirmPassword = request.Password // RegisterRequest doesn't have ConfirmPassword, use Password
            };

            var response = await _httpClient.PostAsJsonAsync(SignUpEndpoint, signUpModel);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("User registration successful for email: {Email}", request.Email);
                return true;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Registration failed for email: {Email}. Status: {StatusCode}, Error: {Error}", 
                    request.Email, response.StatusCode, errorContent);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to register user with email: {Email}", request.Email);
            return false;
        }
    }
    
    public async Task<TokenResponse?> SignInAsync(LoginRequest request)
    {
        try
        {
            // Convert LoginRequest to SignInModel
            var signInModel = new SignInModel
            {
                Email = request.Email,
                Password = request.Password
            };

            var response = await _httpClient.PostAsJsonAsync(SignInEndpoint, signInModel);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Sign-in failed for email: {Email}. Status: {StatusCode}, Error: {Error}", 
                    request.Email, response.StatusCode, errorContent);
                return null;
            }
            
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<TokenResponse>(content, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });
            
            if (result != null)
            {
                _logger.LogInformation("User sign-in successful for email: {Email}", request.Email);
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sign in user with email: {Email}", request.Email);
            return null;
        }
    }

    public async Task<TokenResponse?> RefreshToken(string? refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            _logger.LogWarning("Refresh token is null or empty");
            return null;
        }

        try
        {
            var refreshRequest = new RefreshRequest
            {
                RefreshToken = refreshToken
            };

            var response = await _httpClient.PostAsJsonAsync(RefreshTokenEndpoint, refreshRequest);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Token refresh failed. Status: {StatusCode}, Error: {Error}", 
                    response.StatusCode, errorContent);
                return null;
            }
            
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<TokenResponse>(content, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });
            
            if (result != null)
            {
                _logger.LogInformation("Token refresh successful");
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh token");
            return null;
        }
    }
}