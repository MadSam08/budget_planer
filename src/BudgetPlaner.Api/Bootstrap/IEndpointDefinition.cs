namespace BudgetPlaner.Api.Bootstrap;

public interface IEndpointDefinition
{
    void DefineEndpoints(WebApplication app);
    
    void DefineServices(IServiceCollection services);
}
