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
    /// Initializes a new instance of the BudgetPlanerClient with individual API interfaces
    /// This constructor is used when API interfaces are registered with DI/Refit
    /// </summary>
    public BudgetPlanerClient(
        ICategoriesApi categories,
        IIncomesApi incomes,
        IExpensesApi expenses,
        ILoansApi loans,
        IBudgetsApi budgets,
        IInsightsApi insights,
        ICurrenciesApi currencies,
        IAuthApi auth)
    {
        Categories = categories;
        Incomes = incomes;
        Expenses = expenses;
        Loans = loans;
        Budgets = budgets;
        Insights = insights;
        Currencies = currencies;
        Auth = auth;
    }

    /// <summary>
    /// Creates a new BudgetPlanerClient with the specified base URL
    /// This factory method creates its own HttpClient and Refit instances
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

        // Create API instances using Refit
        var categories = RestService.For<ICategoriesApi>(httpClient);
        var incomes = RestService.For<IIncomesApi>(httpClient);
        var expenses = RestService.For<IExpensesApi>(httpClient);
        var loans = RestService.For<ILoansApi>(httpClient);
        var budgets = RestService.For<IBudgetsApi>(httpClient);
        var insights = RestService.For<IInsightsApi>(httpClient);
        var currencies = RestService.For<ICurrenciesApi>(httpClient);
        var auth = RestService.For<IAuthApi>(httpClient);

        return new BudgetPlanerClient(categories, incomes, expenses, loans, budgets, insights, currencies, auth);
    }
} 