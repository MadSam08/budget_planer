using BudgetPlaner.UI.ApiClients;

namespace BudgetPlaner.UI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthenticatedHttpClient<TInterface, TImplementation>(
        this IServiceCollection services,
        IConfiguration configuration,
        string? relativePath = null)
        where TInterface : class
        where TImplementation : class, TInterface
    {
        services.AddHttpClient<TInterface, TImplementation>(client =>
        {
            var baseUrl = configuration["BaseUrl"]!;
            client.BaseAddress = new Uri(relativePath != null ? $"{baseUrl}/{relativePath}" : baseUrl);
        })
        .AddHttpMessageHandler<AuthenticatedHttpMessageHandler>();

        return services;
    }
} 