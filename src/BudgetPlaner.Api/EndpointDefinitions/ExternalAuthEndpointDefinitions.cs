using System.Security.Claims;
using BudgetPlaner.Api.Bootstrap;
using BudgetPlaner.Api.Constants;
using BudgetPlaner.Infrastructure.DatabaseContext;
using BudgetPlaner.Infrastructure.UnitOfWork;
using BudgetPlaner.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace BudgetPlaner.Api.EndpointDefinitions;

public class ExternalAuthEndpointDefinitions : IEndpointDefinition
{
    private const string BasePath = "/auth";

    public void DefineEndpoints(WebApplication app)
    {
        var group = app.MapGroup(BasePath);
        group.MapGet("/external/{provider}", ChallengeProvider);
        group.MapGet("/external/{provider}/callback", HandleCallback);
    }

    public void DefineServices(IServiceCollection services)
    {
    }

    private static IResult ChallengeProvider(string provider, string? returnUrl)
    {
        var redirectUrl = $"{BasePath}/external/{provider}/callback?returnUrl={Uri.EscapeDataString(returnUrl ?? "/")}";
        var props = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Results.Challenge(props, [provider]);
    }

    private static async Task<IResult> HandleCallback(
        string provider,
        string? returnUrl,
        HttpContext httpContext,
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager,
        IUnitOfWork<BudgetPlanerContext> uow)
    {
        var info = await signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return Results.Redirect("/login");
        }

        var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
        IdentityUser user;
        if (!result.Succeeded)
        {
            var email = info.Principal.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
            user = new IdentityUser { UserName = email, Email = email };
            await userManager.CreateAsync(user);
            await userManager.AddLoginAsync(user, info);
        }
        else
        {
            user = await userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey) ??
                   await userManager.FindByEmailAsync(info.Principal.FindFirstValue(ClaimTypes.Email)!);
        }

        var repo = uow.Repository<UserProfileEntity>();
        var profile = await repo.FirstOrDefaultAsync(x => x.UserId == user.Id);
        if (profile == null)
        {
            await repo.AddAsync(new UserProfileEntity
            {
                UserId = user.Id,
                Email = user.Email ?? string.Empty,
                Provider = info.LoginProvider,
                ProviderUserId = info.ProviderKey,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow
            });
            await uow.Complete();
        }

        await signInManager.SignInAsync(user, false, info.LoginProvider);
        return Results.Redirect(returnUrl ?? "/");
    }
}
