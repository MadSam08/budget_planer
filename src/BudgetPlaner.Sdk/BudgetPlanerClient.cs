using BudgetPlaner.Sdk.Interfaces;
using Refit;

namespace BudgetPlaner.Sdk;

/// <summary>
/// Implementation of the Budget Planer API SDK client
/// </summary>
public class BudgetPlanerClient : IBudgetPlanerClient
{
    public ICategoriesApi Categories { get; }
    public IIncomesApi Incomes { get; }
    public IExpensesApi Expenses { get; }
    public ILoansApi Loans { get; }
    public IBudgetsApi Budgets { get; }
    public IInsightsApi Insights { get; }
    public ICurrenciesApi Currencies { get; }
    public IAuthApi Auth { get; }

    /// <summary>
    /// Initializes a new instance of the BudgetPlanerClient
    /// </summary>
    /// <param name="httpClient">HttpClient configured with base address and authentication</param>
    public BudgetPlanerClient(HttpClient httpClient)
    {
        Categories = RestService.For<ICategoriesApi>(httpClient);
        Incomes = RestService.For<IIncomesApi>(httpClient);
        Expenses = RestService.For<IExpensesApi>(httpClient);
        Loans = RestService.For<ILoansApi>(httpClient);
        Budgets = RestService.For<IBudgetsApi>(httpClient);
        Insights = RestService.For<IInsightsApi>(httpClient);
        Currencies = RestService.For<ICurrenciesApi>(httpClient);
        Auth = RestService.For<IAuthApi>(httpClient);
    }

    /// <summary>
    /// Creates a new BudgetPlanerClient with the specified base URL
    /// </summary>
    /// <param name="baseUrl">The base URL of the Budget Planer API</param>
    /// <param name="accessToken">Optional access token for authentication</param>
    /// <returns>A configured BudgetPlanerClient instance</returns>
    public static IBudgetPlanerClient Create(string baseUrl, string accessToken = null)
    {
        var httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
        
        if (!string.IsNullOrEmpty(accessToken))
        {
            httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        }

        return new BudgetPlanerClient(httpClient);
    }
} 