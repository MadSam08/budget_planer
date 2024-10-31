using BudgetPlaner.UI.ApiClients.Identity;
using BudgetPlaner.UI.Bootstrap;
using BudgetPlaner.UI.Components.Pages.Account;
using BudgetPlaner.UI.Constants.UrlPaths;
using BudgetPlaner.UI.Extensions;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace BudgetPlaner.UI.Controller;

public class IdentityController : IEndpointDefinition
{
    public void DefineEndpoints(WebApplication app)
    {
        app.MapGet(IdentityPaths.SignUp, GetSignUpAsync);
        app.MapPost(IdentityPaths.SignUp, PostSignUpAsync);
    }
    
    private static async Task<IResult> GetSignUpAsync(IIdentityService service, [FromBody] RegisterRequest request)
    {
        var configuration = await service.RegisterAsync(request);
        return Razor.Component<SignUp>(configuration);
    }
    
    private static async Task<IResult> PostSignUpAsync(IIdentityService service, [FromBody] RegisterRequest request)
    {
        var configuration = await service.RegisterAsync(request);
        return Results.Ok(configuration);
    }

    public void DefineServices(IServiceCollection services)
    {
    }
}