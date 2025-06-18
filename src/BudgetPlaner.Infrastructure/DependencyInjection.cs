using BudgetPlaner.Infrastructure.DatabaseContext;
using BudgetPlaner.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BudgetPlaner.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfigurationManager configuration)
    {
        services.AddDbContext<IdentityContext>(
            opts =>
            {
                opts.UseNpgsql(configuration["IdentityDb:DbConnection"],
                    optionsBuilder =>
                    {
                        optionsBuilder.MigrationsAssembly("BudgetPlaner.Api");
                        optionsBuilder.EnableRetryOnFailure();
                    });
            });

        services.AddDbContext<BudgetPlanerContext>(
            opts =>
            {
                opts.UseNpgsql(configuration["BudgetPlanerDb:DbConnection"],
                    optionsBuilder =>
                    {
                        optionsBuilder.MigrationsAssembly("BudgetPlaner.Api");
                        optionsBuilder.EnableRetryOnFailure();
                    });
            });
        
        // Register Unit of Work
        services.AddScoped<IUnitOfWork<BudgetPlanerContext>, UnitOfWork<BudgetPlanerContext>>();
        services.AddScoped<IUnitOfWork<IdentityContext>, UnitOfWork<IdentityContext>>();
        
        return services;
    }
}