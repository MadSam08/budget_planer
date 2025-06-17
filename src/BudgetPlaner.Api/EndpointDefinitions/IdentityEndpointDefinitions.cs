using BudgetPlaner.Api.Bootstrap;
using BudgetPlaner.Api.Constants;
using BudgetPlaner.Api.Constants.EndpointNames;
using Microsoft.AspNetCore.Identity;

namespace BudgetPlaner.Api.EndpointDefinitions;

public class IdentityEndpointDefinitions : IEndpointDefinition
{
    public void DefineEndpoints(WebApplication app)
    {
        app.MapGroup(ApiEndpoints.Auth.BasePath)
            .WithTags(SwaggerTags.AccountTag)
            .MapIdentityApi<IdentityUser>();
    }

    public void DefineServices(IServiceCollection services)
    {
    }
}