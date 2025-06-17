namespace BudgetPlaner.Api.Constants.EndpointNames;

/// <summary>
/// RESTful API endpoints following the pattern: api/{resource}/{id?}/{sub-resource?}
/// All endpoints use a consistent base path and follow REST conventions
/// </summary>
public static class ApiEndpoints
{
    private const string ApiBase = "api";

    public static class Categories
    {
        private const string Base = $"{ApiBase}/categories";

        public const string GetAll = Base;                    // GET    /api/categories
        public const string Get = $"{Base}/{{id}}";           // GET    /api/categories/{id}
        public const string Create = Base;                    // POST   /api/categories
        public const string Update = $"{Base}/{{id}}";        // PUT    /api/categories/{id}
        public const string Delete = $"{Base}/{{id}}";        // DELETE /api/categories/{id}
        public const string Restore = $"{Base}/{{id}}";       // PATCH  /api/categories/{id} (with restore action in body)
    }

    public static class Incomes
    {
        private const string Base = $"{ApiBase}/incomes";

        public const string GetAll = Base;                    // GET    /api/incomes
        public const string Get = $"{Base}/{{id}}";           // GET    /api/incomes/{id}
        public const string Create = Base;                    // POST   /api/incomes
        public const string Update = $"{Base}/{{id}}";        // PUT    /api/incomes/{id}
        public const string Delete = $"{Base}/{{id}}";        // DELETE /api/incomes/{id}
    }

    public static class Expenses
    {
        private const string Base = $"{ApiBase}/expenses";

        public const string GetAll = Base;                    // GET    /api/expenses
        public const string Get = $"{Base}/{{id}}";           // GET    /api/expenses/{id}
        public const string Create = Base;                    // POST   /api/expenses
        public const string Update = $"{Base}/{{id}}";        // PUT    /api/expenses/{id}
        public const string Delete = $"{Base}/{{id}}";        // DELETE /api/expenses/{id}
    }

    public static class Loans
    {
        private const string Base = $"{ApiBase}/loans";

        public const string GetAll = Base;                           // GET    /api/loans
        public const string Get = $"{Base}/{{id}}";                  // GET    /api/loans/{id}
        public const string Create = Base;                           // POST   /api/loans
        public const string Update = $"{Base}/{{id}}";               // PUT    /api/loans/{id}
        public const string Delete = $"{Base}/{{id}}";               // DELETE /api/loans/{id}
        
        // Nested resources
        public const string GetInterestRates = $"{Base}/{{id}}/interest-rates";     // GET    /api/loans/{id}/interest-rates
        public const string GenerateInterestRates = $"{Base}/{{id}}/interest-rates"; // POST   /api/loans/{id}/interest-rates
    }

    public static class Budgets
    {
        private const string Base = $"{ApiBase}/budgets";

        public const string GetAll = Base;                    // GET    /api/budgets
        public const string Get = $"{Base}/{{id}}";           // GET    /api/budgets/{id}
        public const string Create = Base;                    // POST   /api/budgets
        public const string Update = $"{Base}/{{id}}";        // PUT    /api/budgets/{id}
        public const string Delete = $"{Base}/{{id}}";        // DELETE /api/budgets/{id}

        // Nested resources - Budget Categories
        public const string GetCategories = $"{Base}/{{budgetId}}/categories";              // GET    /api/budgets/{budgetId}/categories
        public const string AddCategory = $"{Base}/{{budgetId}}/categories";                // POST   /api/budgets/{budgetId}/categories
        public const string UpdateCategory = $"{Base}/{{budgetId}}/categories/{{categoryId}}"; // PUT    /api/budgets/{budgetId}/categories/{categoryId}
        public const string RemoveCategory = $"{Base}/{{budgetId}}/categories/{{categoryId}}"; // DELETE /api/budgets/{budgetId}/categories/{categoryId}

        // Sub-resources - Budget Analysis
        public const string GetUtilization = $"{Base}/{{id}}/utilization";    // GET /api/budgets/{id}/utilization
        public const string GetOverBudget = $"{Base}/{{id}}/over-budget";     // GET /api/budgets/{id}/over-budget
    }

    public static class Insights
    {
        private const string Base = $"{ApiBase}/insights";

        public const string GetAll = Base;                    // GET    /api/insights
        public const string Get = $"{Base}/{{id}}";           // GET    /api/insights/{id}
        public const string Create = Base;                    // POST   /api/insights (generate)

        // Actions on specific insights
        public const string MarkAsRead = $"{Base}/{{id}}/read";        // PUT /api/insights/{id}/read
        public const string MarkActionTaken = $"{Base}/{{id}}/actions"; // PUT /api/insights/{id}/actions

        // Analysis sub-resources
        public const string SpendingPatterns = $"{Base}/spending-patterns";       // GET /api/insights/spending-patterns
        public const string SavingsOpportunities = $"{Base}/savings-opportunities"; // GET /api/insights/savings-opportunities
        public const string BudgetPerformance = $"{Base}/budget-performance";     // GET /api/insights/budget-performance
    }

    public static class Currencies
    {
        private const string Base = $"{ApiBase}/currencies";

        public const string GetAll = Base;                    // GET    /api/currencies
        public const string Get = $"{Base}/{{id}}";           // GET    /api/currencies/{id}
        public const string Create = Base;                    // POST   /api/currencies
        public const string Update = $"{Base}/{{id}}";        // PUT    /api/currencies/{id}
        public const string Delete = $"{Base}/{{id}}";        // DELETE /api/currencies/{id}
        public const string Restore = $"{Base}/{{id}}";       // PATCH  /api/currencies/{id} (with restore action in body)
    }

    public static class Auth
    {
        private const string Base = $"{ApiBase}/auth";
        
        // Base path for MapIdentityApi
        public const string BasePath = Base;

        // Session management
        public const string SignIn = $"{Base}/signin";        // POST /api/auth/signin
        public const string SignUp = $"{Base}/signup";        // POST /api/auth/signup
        public const string SignOut = $"{Base}/signout";      // POST /api/auth/signout
        public const string RefreshToken = $"{Base}/refresh"; // POST /api/auth/refresh

        // User profile
        public const string GetProfile = $"{Base}/profile";   // GET  /api/auth/profile
        public const string UpdateProfile = $"{Base}/profile"; // PUT  /api/auth/profile

        // External authentication
        public const string ExternalProviders = $"{Base}/external";                    // GET  /api/auth/external
        public const string ExternalSignIn = $"{Base}/external/{{provider}}";         // GET  /api/auth/external/{provider}
        public const string ExternalCallback = $"{Base}/external/{{provider}}/callback"; // GET  /api/auth/external/{provider}/callback
    }
} 