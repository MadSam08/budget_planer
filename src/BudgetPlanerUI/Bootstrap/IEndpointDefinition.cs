namespace BudgetPlaner.UI.Bootstrap;

public interface IEndpointDefinition
{
    void DefineEndpoints(WebApplication app);
    
    void DefineServices(IServiceCollection services);
}
