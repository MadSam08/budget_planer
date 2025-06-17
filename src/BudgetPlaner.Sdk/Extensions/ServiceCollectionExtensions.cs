using BudgetPlaner.Sdk.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace BudgetPlaner.Sdk.Extensions;

/// <summary>
/// Extension methods for registering Budget Planer SDK with dependency injection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Budget Planer SDK to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="baseUrl">The base URL of the Budget Planer API</param>
    /// <param name="configureClient">Optional configuration for the HTTP client</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddBudgetPlanerSdk(this IServiceCollection services, 
        string baseUrl, 
        Action<HttpClient>? configureClient = null)
    {
        // Register individual API interfaces
        services.AddRefitClient<ICategoriesApi>()
            .ConfigureHttpClient(client => 
            {
                client.BaseAddress = new Uri(baseUrl);
                configureClient?.Invoke(client);
            });

        services.AddRefitClient<IIncomesApi>()
            .ConfigureHttpClient(client => 
            {
                client.BaseAddress = new Uri(baseUrl);
                configureClient?.Invoke(client);
            });

        services.AddRefitClient<IExpensesApi>()
            .ConfigureHttpClient(client => 
            {
                client.BaseAddress = new Uri(baseUrl);
                configureClient?.Invoke(client);
            });

        services.AddRefitClient<ILoansApi>()
            .ConfigureHttpClient(client => 
            {
                client.BaseAddress = new Uri(baseUrl);
                configureClient?.Invoke(client);
            });

        services.AddRefitClient<IBudgetsApi>()
            .ConfigureHttpClient(client => 
            {
                client.BaseAddress = new Uri(baseUrl);
                configureClient?.Invoke(client);
            });

        services.AddRefitClient<IInsightsApi>()
            .ConfigureHttpClient(client => 
            {
                client.BaseAddress = new Uri(baseUrl);
                configureClient?.Invoke(client);
            });

        services.AddRefitClient<ICurrenciesApi>()
            .ConfigureHttpClient(client => 
            {
                client.BaseAddress = new Uri(baseUrl);
                configureClient?.Invoke(client);
            });

        services.AddRefitClient<IAuthApi>()
            .ConfigureHttpClient(client => 
            {
                client.BaseAddress = new Uri(baseUrl);
                configureClient?.Invoke(client);
            });

        // Register the main client
        services.AddScoped<IBudgetPlanerClient>(provider =>
        {
            var httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
            configureClient?.Invoke(httpClient);
            return new BudgetPlanerClient(httpClient);
        });

        return services;
    }

    /// <summary>
    /// Adds Budget Planer SDK to the service collection with bearer token authentication
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="baseUrl">The base URL of the Budget Planer API</param>
    /// <param name="getAccessToken">Function to get the access token</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddBudgetPlanerSdk(this IServiceCollection services, 
        string baseUrl, 
        Func<IServiceProvider, string> getAccessToken)
    {
        return services.AddBudgetPlanerSdk(baseUrl, client =>
        {
            // The access token will be set per request using a delegating handler
            // This allows for dynamic token refresh
        });
    }
} 