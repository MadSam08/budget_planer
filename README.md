# Budget Planner - Full Stack Personal Finance Management System

A comprehensive personal finance management system built with .NET 9, featuring a REST API, Blazor Server UI, and a strongly-typed SDK. The system follows Domain Driven Design (DDD) and Clean Architecture principles with advanced authentication and AI-powered financial insights.

## 📋 Table of Contents

- [🚀 Features](#-features)
- [🏗️ Architecture](#️-architecture)
- [🧩 Project Structure](#-project-structure)
- [🔌 API Documentation](#-api-documentation)
- [💻 Blazor UI & Authentication](#-blazor-ui--authentication)
- [📦 SDK Integration](#-sdk-integration)
- [🛠️ Development Setup](#️-development-setup)
- [🚀 Deployment](#-deployment)
- [📈 Roadmap](#-roadmap)

## 🚀 Features

### Core Functionality
- **Income & Expense Tracking**: Record and categorize all financial transactions
- **Budget Management**: Create and monitor budgets with category-wise allocations
- **Savings Goals**: Set and track progress towards financial goals
- **Loan Management**: Track loans with amortization schedules and early payoff calculations
- **Financial Insights**: AI-powered suggestions for saving money and optimizing finances

### Advanced Features
- **Smart Spending Analysis**: Detect spending patterns and unusual expenses
- **Budget Optimization**: Recommendations for budget reallocation
- **Loan Optimization**: Calculate savings from extra payments and early payoffs
- **Savings Opportunities**: Identify areas where money can be saved
- **Monthly Financial Reports**: Automated insights and recommendations

### Technical Features
- **REST API**: Comprehensive API following REST principles
- **Blazor Server UI**: Modern web interface with real-time updates
- **Type-Safe SDK**: Refit-based SDK for easy API consumption
- **Bearer Token Authentication**: Secure JWT-based authentication with automatic refresh
- **Real-time Updates**: SignalR integration for live data updates

## 🏗️ Architecture

The project follows Clean Architecture with these layers:

```
BudgetPlanner/
├── src/
│   ├── BudgetPlaner.Domain/          # Domain entities and business rules
│   ├── BudgetPlaner.Application/     # Use cases and business logic
│   ├── BudgetPlaner.Infrastructure/  # Data access and external services
│   ├── BudgetPlaner.Api/            # Web API endpoints
│   ├── BudgetPlaner.Contracts/      # DTOs and API contracts
│   ├── BudgetPlaner.Sdk/            # Client SDK for API consumption
│   └── BudgetPlanerUI/              # Blazor Server application
└── test/
    └── BudgetPlaner.Tests/          # Unit and integration tests
```

### Domain Model

#### Core Entities
- **Income**: Track various income sources (salary, freelance, investments, etc.)
- **Spending**: Record expenses with merchant, location, and categorization
- **Budget**: Monthly/yearly budget plans with category allocations
- **Category**: Organize transactions by type (Income, Expense, Default)
- **Currency**: Multi-currency support
- **SavingsGoal**: Financial targets with progress tracking
- **Loan**: Loan management with payment schedules
- **FinancialInsight**: AI-generated suggestions and alerts

## 🧩 Project Structure

### API Layer (`BudgetPlaner.Api`)
- **Minimal APIs**: Lightweight, fast endpoint definitions following REST principles
- **Endpoint Definitions**: Organized by domain (Categories, Budgets, Loans, etc.)
- **Swagger Integration**: Comprehensive API documentation
- **Authentication**: JWT Bearer token support with automatic refresh
- **Validation**: Request/response validation
- **Error Handling**: Consistent error responses

### Contracts (`BudgetPlaner.Contracts`)
- **API Models**: Request/response DTOs shared between API and SDK
- **Enums**: Shared enumeration types
- **Settings**: Configuration models
- **Claims**: Authentication claim definitions

### SDK (`BudgetPlaner.Sdk`)
Type-safe client library featuring:
- **Refit Integration**: Strongly-typed HTTP clients
- **Authentication Support**: Built-in Bearer token handling
- **Dependency Injection**: Easy service registration
- **Contract Models**: Consistent with API contracts

### UI (`BudgetPlanerUI`)
Modern Blazor Server application with:
- **Component Architecture**: Reusable UI components
- **MudBlazor**: Material Design component library
- **Authentication**: Secure user authentication with automatic token refresh
- **SDK Integration**: Type-safe API access
- **Responsive Design**: Mobile-friendly interface

## 🔌 API Documentation

### REST Endpoints

#### Categories
```
GET    /api/categories                 # Get all categories
GET    /api/categories/{id}            # Get category by ID
POST   /api/categories                 # Create category
PUT    /api/categories/{id}            # Update category
DELETE /api/categories/{id}            # Delete category
PATCH  /api/categories/{id}/restore    # Restore deleted category
```

#### Income & Expenses
```
GET    /api/incomes                    # Get income records
POST   /api/incomes                    # Create income record
PUT    /api/incomes/{id}               # Update income
DELETE /api/incomes/{id}               # Delete income

GET    /api/expenses                   # Get expense records
POST   /api/expenses                   # Create expense record
PUT    /api/expenses/{id}              # Update expense
DELETE /api/expenses/{id}              # Delete expense
```

#### Budget Management
```
GET    /api/budgets                    # Get user budgets
POST   /api/budgets                    # Create budget
GET    /api/budgets/{id}               # Get budget details
PUT    /api/budgets/{id}               # Update budget
DELETE /api/budgets/{id}               # Delete budget

GET    /api/budgets/{id}/categories    # Get budget categories
POST   /api/budgets/{id}/categories    # Add category to budget
PUT    /api/budgets/{budgetId}/categories/{categoryId}  # Update budget category
DELETE /api/budgets/{budgetId}/categories/{categoryId} # Remove category

GET    /api/budgets/{id}/utilization   # Get budget utilization
GET    /api/budgets/{id}/over-budget   # Get over-budget categories
```

#### Loan Management
```
GET    /api/loans                      # Get user loans
POST   /api/loans                      # Create loan
GET    /api/loans/{id}                 # Get loan details
PUT    /api/loans/{id}                 # Update loan
DELETE /api/loans/{id}                 # Delete loan

GET    /api/loans/{id}/interest-rates  # Get loan interest rates
POST   /api/loans/{id}/generate-rates  # Generate interest rate schedule
```

#### Financial Insights
```
GET    /api/insights                   # Get all insights
GET    /api/insights/{id}              # Get specific insight
POST   /api/insights                   # Generate new insights

PUT    /api/insights/{id}/read         # Mark insight as read
PUT    /api/insights/{id}/action-taken # Mark action taken

GET    /api/insights/spending-patterns # Analyze spending patterns
GET    /api/insights/savings-opportunities # Find savings opportunities
GET    /api/insights/budget-performance # Analyze budget performance
```

#### Authentication
```
POST   /api/auth/signin                # User login
POST   /api/auth/signup                # User registration
POST   /api/auth/signout               # User logout
POST   /api/auth/refresh               # Refresh access token

GET    /api/auth/profile               # Get user profile
PUT    /api/auth/profile               # Update user profile

GET    /api/auth/external-providers    # Get external auth providers
GET    /api/auth/external-signin       # External provider login
GET    /api/auth/external-callback     # External auth callback
```

#### Currencies
```
GET    /api/currencies                 # Get all currencies
GET    /api/currencies/{id}            # Get currency by ID
POST   /api/currencies                 # Create currency
PUT    /api/currencies/{id}            # Update currency
DELETE /api/currencies/{id}            # Delete currency
PATCH  /api/currencies/{id}/restore    # Restore deleted currency
```

## 💻 Blazor UI & Authentication

### Authentication System

The Blazor UI implements a comprehensive Bearer token authentication system designed specifically for Blazor Server:

#### Key Components

1. **`BudgetPlanerSdkService`** - Manages SDK client creation with authentication
2. **`TokenRefreshService`** - Handles automatic token refresh
3. **`AuthTokenHandler`** - HTTP message handler for automatic token injection
4. **`UserRevalidatingState`** - Custom authentication state provider

#### Authentication Flow

1. **User Login** → JWT tokens stored in claims
2. **API Request** → Token automatically extracted and attached
3. **Token Expiry Check** → Automatic refresh if needed (2-minute buffer)
4. **Request Sent** → With valid Bearer token
5. **401 Response** → Automatic token refresh and retry

#### Blazor Server Authentication Pattern

Following Microsoft's recommendations, the system uses this pattern:

```razor
@using BudgetPlaner.Sdk
@using BudgetPlaner.UI.Services
@using Microsoft.AspNetCore.Components.Authorization
@inherits OwningComponentBase
@inject AuthenticationStateProvider AuthenticationStateProvider

@code {
    private IBudgetPlanerSdkService? _sdkService;

    protected override async Task OnInitializedAsync()
    {
        // Get SDK service using OwningComponentBase.ScopedServices
        // This ensures correct, initialized instance for this component scope
        _sdkService = ScopedServices.GetRequiredService<IBudgetPlanerSdkService>();
        await LoadData();
    }

    private async Task LoadData()
    {
        try
        {
            // Pass AuthenticationStateProvider to get authenticated client
            var client = await _sdkService!.GetClientAsync(AuthenticationStateProvider);
            var data = await client.Categories.GetCategoriesAsync();
            // Process data...
        }
        catch (UnauthorizedAccessException)
        {
            // Handle authentication errors
            Logger.LogWarning("User is not authenticated");
            Snackbar.Add("Authentication required. Please log in.", Severity.Warning);
        }
        catch (Exception ex)
        {
            // Handle other errors
            Logger.LogError(ex, "API operation failed");
            Snackbar.Add("Operation failed. Please try again.", Severity.Error);
        }
    }
}
```

#### Why This Pattern?

**Problem**: Using `AuthenticationStateProvider.GetAuthenticationStateAsync()` in HTTP message handlers causes:
```
System.InvalidOperationException: Do not call GetAuthenticationStateAsync outside of the DI scope for a Razor component.
```

**Solution**: 
- ✅ **Inject in Components**: `AuthenticationStateProvider` injected at component level
- ✅ **Pass to Services**: Pass as parameter to avoid DI scope issues
- ✅ **OwningComponentBase**: Use for component-scoped services
- ✅ **No Service Injection**: Don't inject `AuthenticationStateProvider` in services

#### Token Management

```csharp
public class BudgetPlanerClaims
{
    public const string AccessToken = "AccessToken";   // JWT access token
    public const string RefreshToken = "RefreshToken"; // Refresh token
    public const string ExpiresIn = "ExpiresIn";       // Token expiration time
    public const string ExpiresAt = "ExpiresAt";       // Exact expiry timestamp
}
```

#### Security Features
- 🔒 **Secure HttpOnly Cookies**: Token storage in secure cookies
- 🔒 **SameSite=Strict**: CSRF protection
- 🔒 **Automatic Expiration**: Proactive token refresh
- 🔒 **Thread-Safe Refresh**: Prevents concurrent refresh attempts
- 🔒 **Automatic Cleanup**: Logout on authentication failure

### UI Features

- ✅ **Automatic Token Management**: Bearer tokens automatically added to requests
- ✅ **Token Refresh**: Automatic refresh with 2-minute buffer
- ✅ **Error Handling**: Comprehensive error handling and user feedback
- ✅ **Component Scoping**: Proper DI scoping following Microsoft guidelines
- ✅ **Type Safety**: Strongly-typed API interactions via SDK
- ✅ **Loading States**: User-friendly loading indicators
- ✅ **Real-time Updates**: Live data synchronization

## 📦 SDK Integration

The BudgetPlaner SDK provides a strongly-typed, authentication-aware client for the API.

### Setup

#### 1. Project Reference
```xml
<ProjectReference Include="..\BudgetPlaner.Sdk\BudgetPlaner.Sdk.csproj" />
```

#### 2. Service Registration
```csharp
// In Program.cs
builder.Services.AddScoped<IBudgetPlanerSdkService, BudgetPlanerSdkService>();
```

#### 3. Component Usage Pattern

```csharp
// In Blazor components following the authentication pattern
var client = await _sdkService.GetClientAsync(AuthenticationStateProvider);

// All APIs are strongly typed
var categories = await client.Categories.GetCategoriesAsync();
var budgets = await client.Budgets.GetBudgetsAsync();
var insights = await client.Insights.GetInsightsAsync();
```

### Available APIs

The SDK provides access to all API endpoints:

- **Categories**: `client.Categories.*` - Category management
- **Incomes**: `client.Incomes.*` - Income tracking
- **Expenses**: `client.Expenses.*` - Expense management
- **Loans**: `client.Loans.*` - Loan management with interest calculations
- **Budgets**: `client.Budgets.*` - Budget creation and monitoring
- **Insights**: `client.Insights.*` - Financial insights and analytics
- **Currencies**: `client.Currencies.*` - Currency management
- **Auth**: `client.Auth.*` - Authentication operations

### SDK Features

- **Type Safety**: Compile-time validation of API contracts
- **Automatic Authentication**: Bearer tokens handled automatically
- **Error Handling**: Consistent exception handling
- **Refit Integration**: Strongly-typed HTTP client generation
- **Contract Consistency**: Uses same models as API

### Migration from Direct HTTP Calls

The SDK replaces direct HTTP calls with strongly-typed methods:

```csharp
// Before: Direct HTTP calls
var response = await httpClient.GetAsync("/api/categories");
var json = await response.Content.ReadAsStringAsync();
var categories = JsonSerializer.Deserialize<CategoryModel[]>(json);

// After: SDK usage
var categories = await client.Categories.GetCategoriesAsync();
```

## 🛠️ Development Setup

### Prerequisites
- .NET 9 SDK
- SQL Server or SQL Server Express
- Visual Studio 2022 or VS Code
- Git

### Getting Started

1. **Clone Repository**
   ```bash
   git clone https://github.com/your-repo/budget-planner.git
   cd budget-planner
   ```

2. **Setup Database**
   ```bash
   # Update connection string in appsettings.json
   dotnet ef database update --project src/BudgetPlaner.Infrastructure
   ```

3. **Configure Settings**
   ```json
   // appsettings.json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=.;Database=BudgetPlaner;Trusted_Connection=true;"
     },
     "BaseUrl": "https://localhost:7240"
   }
   ```

4. **Run Applications**
   ```bash
   # Terminal 1: Start API
   dotnet run --project src/BudgetPlaner.Api

   # Terminal 2: Start UI
   dotnet run --project src/BudgetPlanerUI
   ```

5. **Access Applications**
   - **API**: `https://localhost:7240`
   - **UI**: `https://localhost:7241`
   - **Swagger**: `https://localhost:7240/swagger`

### Development Workflow

#### API Development
1. Add entities in `BudgetPlaner.Domain`
2. Update contracts in `BudgetPlaner.Contracts`
3. Implement services in `BudgetPlaner.Application`
4. Create endpoints in `BudgetPlaner.Api`
5. Update SDK interfaces in `BudgetPlaner.Sdk`

#### UI Development
1. Create Blazor components in `BudgetPlanerUI/Components`
2. Use SDK for API interactions
3. Follow authentication patterns with `OwningComponentBase`
4. Implement error handling and user feedback

### Testing

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## 🚀 Deployment

### Docker Support
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/BudgetPlaner.Api/BudgetPlaner.Api.csproj", "src/BudgetPlaner.Api/"]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BudgetPlaner.Api.dll"]
```

### Azure Deployment
- **Azure App Service**: Host API and UI
- **Azure SQL Database**: Data storage
- **Azure Key Vault**: Secrets management
- **Azure Application Insights**: Monitoring

## 📈 Roadmap

### Phase 1: Foundation ✅
- [x] Core domain models and entities
- [x] REST API with comprehensive endpoints
- [x] Blazor Server UI with authentication
- [x] Type-safe SDK with Refit
- [x] JWT authentication with automatic refresh
- [x] Proper Blazor Server authentication patterns

### Phase 2: Enhanced Features (In Progress)
- [x] Budget management system
- [x] Financial insights framework
- [x] Category management with restore functionality
- [ ] Savings goals implementation
- [ ] Advanced loan calculations and optimization

### Phase 3: AI & Analytics
- [ ] AI-powered spending analysis
- [ ] Predictive budget recommendations
- [ ] Automated financial insights generation
- [ ] Custom reporting and dashboards
- [ ] Data export functionality

### Phase 4: Integrations & Mobile
- [ ] Bank API integrations
- [ ] Email notifications and alerts
- [ ] Mobile app (Blazor Hybrid)
- [ ] Third-party service integrations
- [ ] Progressive Web App (PWA) features

### Phase 5: Enterprise Features
- [ ] Multi-tenancy support
- [ ] Advanced user management
- [ ] Audit trails and compliance
- [ ] Performance optimization
- [ ] Comprehensive monitoring and logging

## 🛠️ Technology Stack

- **.NET 9**: Latest .NET framework
- **Entity Framework Core**: ORM for data access
- **SQL Server**: Primary database
- **Minimal APIs**: Lightweight, fast API endpoints
- **Blazor Server**: Interactive web UI with real-time updates
- **MudBlazor**: Material Design component library
- **Refit**: Type-safe HTTP client generation
- **JWT**: JSON Web Token authentication
- **SignalR**: Real-time communication
- **Docker**: Containerization support
- **Azure**: Cloud deployment platform

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📞 Support

- Create an issue on GitHub for bugs and feature requests
- Check the comprehensive documentation above
- Review the SDK integration patterns
- Follow the Blazor Server authentication guidelines

---

**Budget Planner** - Your comprehensive, type-safe personal finance management solution 💰📊✨
