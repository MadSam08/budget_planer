using System.Security.Claims;

namespace BudgetPlaner.Api.Extensions;

public static class HttpContextAccessorExtensions
{
    public static string? GetUserIdFromClaims(this IHttpContextAccessor httpContextAccessor)
    {
        return httpContextAccessor.HttpContext?.User.Claims
            .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
    }
}