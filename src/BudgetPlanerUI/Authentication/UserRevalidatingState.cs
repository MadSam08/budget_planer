using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;

namespace BudgetPlaner.UI.Authentication
{
    // This is a server-side AuthenticationStateProvider that revalidates the security stamp for the connected user
    // every 30 minutes an interactive circuit is connected.
    internal sealed class UserRevalidatingState(
        ILoggerFactory loggerFactory)
        : RevalidatingServerAuthenticationStateProvider(loggerFactory)
    {
        protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);

        protected override Task<bool> ValidateAuthenticationStateAsync(
            AuthenticationState authenticationState, CancellationToken cancellationToken)
        {
            var user = authenticationState.User;
            if (!(user?.Identity?.IsAuthenticated ?? false)) return Task.FromResult(false);
            var expiryDateClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Expiration);
            var expiryDateValue = expiryDateClaim?.Value ?? "";

            if (string.IsNullOrEmpty(expiryDateValue)) return Task.FromResult(false);

            return !DateTimeOffset.TryParse(expiryDateValue, CultureInfo.InvariantCulture, out var expiryDate)
                ? Task.FromResult(false)
                : Task.FromResult(expiryDate > DateTimeOffset.UtcNow);
        }
    }
}