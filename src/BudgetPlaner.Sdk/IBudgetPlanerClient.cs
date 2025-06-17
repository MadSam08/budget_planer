using BudgetPlaner.Sdk.Interfaces;

namespace BudgetPlaner.Sdk;

/// <summary>
/// Main client interface for Budget Planer API SDK
/// Provides access to all API endpoints through typed interfaces
/// </summary>
public interface IBudgetPlanerClient
{
    /// <summary>
    /// API for managing categories
    /// </summary>
    ICategoriesApi Categories { get; }

    /// <summary>
    /// API for managing incomes
    /// </summary>
    IIncomesApi Incomes { get; }

    /// <summary>
    /// API for managing expenses
    /// </summary>
    IExpensesApi Expenses { get; }

    /// <summary>
    /// API for managing loans
    /// </summary>
    ILoansApi Loans { get; }

    /// <summary>
    /// API for managing budgets and budget categories
    /// </summary>
    IBudgetsApi Budgets { get; }

    /// <summary>
    /// API for financial insights and analysis
    /// </summary>
    IInsightsApi Insights { get; }

    /// <summary>
    /// API for managing currencies
    /// </summary>
    ICurrenciesApi Currencies { get; }

    /// <summary>
    /// API for authentication and user management
    /// </summary>
    IAuthApi Auth { get; }
} 