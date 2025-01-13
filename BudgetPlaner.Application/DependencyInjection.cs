using Microsoft.Extensions.DependencyInjection;

namespace BudgetPlaner.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(o =>
        {
            o.RegisterServicesFromAssemblyContaining(typeof(DependencyInjection));
        });
        return services;
    }
}