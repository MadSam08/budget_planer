using BudgetPlaner.Sdk.Constants;
using BudgetPlaner.Contracts.Api.Identity;
using BudgetPlaner.Contracts.Api.Profile;
using BudgetPlaner.Contracts.UI.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Refit;
using RefreshRequest = Microsoft.AspNetCore.Identity.Data.RefreshRequest;

namespace BudgetPlaner.Sdk.Interfaces;

[Headers("Content-Type: application/json; charset=UTF-8", "Accept: application/json")]
public interface IAuthApi
{
    [Post(ApiEndpoints.Auth.SignIn)]
    Task<TokenResponse> SignInAsync([Body] SignInModel request);

    [Post(ApiEndpoints.Auth.SignUp)]
    Task<TokenResponse> SignUpAsync([Body] SignUpModel request);

    [Post(ApiEndpoints.Auth.SignOut)]
    Task SignOutAsync();

    [Post(ApiEndpoints.Auth.RefreshToken)]
    Task<TokenResponse> RefreshTokenAsync([Body] RefreshRequest request);

    [Get(ApiEndpoints.Auth.GetProfile)]
    [Headers("Authorization: Bearer")]
    Task<UserProfileModel> GetProfileAsync();

    [Put(ApiEndpoints.Auth.UpdateProfile)]
    [Headers("Authorization: Bearer")]
    Task<UserProfileModel> UpdateProfileAsync([Body] UserProfileModel request);

    // External Authentication
    [Get(ApiEndpoints.Auth.ExternalProviders)]
    Task<IEnumerable<string>> GetExternalProvidersAsync();

    [Get(ApiEndpoints.Auth.ExternalSignIn)]
    Task<string> GetExternalSignInUrlAsync(string provider, [Query] string returnUrl = null);

    [Get(ApiEndpoints.Auth.ExternalCallback)]
    Task<TokenResponse> HandleExternalCallbackAsync(string provider, [Query] string code, [Query] string state = null);
} 