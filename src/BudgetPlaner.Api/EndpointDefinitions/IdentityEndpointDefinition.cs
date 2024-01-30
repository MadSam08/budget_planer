using BudgetPlaner.Api.Bootstrap;
using BudgetPlaner.Api.Constants.EndpointNames;
using Microsoft.AspNetCore.Identity;

namespace BudgetPlaner.Api.EndpointDefinitions;

public class IdentityEndpointDefinition : IEndpointDefinition
{
    private const string BasePath = $"{EndpointNames.BudgetBasePath}/{EndpointNames.AccountPath}";
    
    public void DefineEndpoints(WebApplication app)
    {
        app.MapGroup(BasePath)
            .WithTags(SwaggerTags.AccountTag)
            .MapIdentityApi<IdentityUser>();
    }

    public void DefineServices(IServiceCollection services)
    {
    }
}