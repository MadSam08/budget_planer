using BudgetPlaner.Sdk;
using BudgetPlaner.Sdk.Interfaces;

namespace BudgetPlaner.UI.Services;

/// <summary>
/// Service interface for the Budget Planer SDK client.
/// With the new Refit registration pattern, this is simply an alias for IBudgetPlanerClient
/// since authentication is handled automatically by the AuthTokenProvider.
/// </summary>
public interface IBudgetPlanerSdkService : IBudgetPlanerClient
{
}

/// <summary>
/// Implementation that delegates to the injected IBudgetPlanerClient.
/// The actual client is configured with Refit and automatic authentication.
/// </summary>
public class BudgetPlanerSdkService : IBudgetPlanerSdkService
{
    private readonly IBudgetPlanerClient _client;

    public BudgetPlanerSdkService(IBudgetPlanerClient client)
    {
        _client = client;
    }

    // Delegate all SDK client properties to the injected client
    public ICategoriesApi Categories => _client.Categories;
    public IIncomesApi Incomes => _client.Incomes;
    public IExpensesApi Expenses => _client.Expenses;
    public ILoansApi Loans => _client.Loans;
    public IBudgetsApi Budgets => _client.Budgets;
    public IInsightsApi Insights => _client.Insights;
    public ICurrenciesApi Currencies => _client.Currencies;
    public IAuthApi Auth => _client.Auth;
} 