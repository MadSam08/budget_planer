using BudgetPlaner.Api.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace BudgetPlaner.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void ApplyMigration(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        try
        {
            RunMigrations<BudgetPlanerContext>(services);
            RunMigrations<IdentityContext>(services);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError("Failed to apply migration with {Exception}", ex);
        }
    }
    
    private static void RunMigrations<TContext>(IServiceProvider services) where TContext : DbContext
    {
        var context = services.GetRequiredService<TContext>();
        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }
    }
}