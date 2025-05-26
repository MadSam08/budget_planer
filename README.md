# Budget Planner API

A comprehensive personal finance management API built with .NET 8, following Domain Driven Design (DDD) and Clean Architecture principles.

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

## 🏗️ Architecture

The project follows Clean Architecture with these layers:

```
├── BudgetPlaner.Domain/          # Domain entities and business rules
├── BudgetPlaner.Application/     # Use cases and business logic
├── BudgetPlaner.Infrastructure/  # Data access and external services
├── BudgetPlaner.Api/            # Web API endpoints
├── BudgetPlaner.Contracts/      # DTOs and API contracts
└── BudgetPlaner.Models/         # Shared models
```

## 📊 Domain Model

### Core Entities
- **Income**: Track various income sources (salary, freelance, investments, etc.)
- **Spending**: Record expenses with merchant, location, and categorization
- **Budget**: Monthly/yearly budget plans with category allocations
- **SavingsGoal**: Financial targets with progress tracking
- **Loan**: Loan management with payment schedules
- **FinancialInsight**: AI-generated suggestions and alerts

### Key Features by Entity

#### Income Management
- Multiple income types (salary, freelance, investment, etc.)
- Recurring income tracking
- Tax categorization
- Source tracking

#### Expense Tracking
- Detailed spending records with merchant and location
- Multiple payment methods (cash, card, digital wallet)
- Recurring expense identification
- Tag-based categorization

#### Budget Planning
- Flexible budget periods (monthly, quarterly, yearly)
- Category-wise budget allocation
- Real-time budget utilization tracking
- Over-budget alerts

#### Savings Goals
- Target amount and deadline tracking
- Progress visualization
- Priority-based goal management
- Contribution history

#### Loan Management
- Multiple loan types (personal, mortgage, auto, etc.)
- Amortization schedule generation
- Early payoff calculations
- Payment tracking and history

## 🔧 API Endpoints

### Budget Management
```
POST   /api/budgets                    # Create budget
GET    /api/budgets                    # Get user budgets
GET    /api/budgets/{id}               # Get budget details
PUT    /api/budgets/{id}               # Update budget
DELETE /api/budgets/{id}               # Delete budget

POST   /api/budgets/{id}/categories    # Add category to budget
PUT    /api/budgets/categories/{id}    # Update budget category
DELETE /api/budgets/categories/{id}    # Remove category from budget

GET    /api/budgets/{id}/utilization   # Get budget utilization
GET    /api/budgets/{id}/over-budget   # Get over-budget categories
```

### Financial Insights
```
GET    /api/insights                   # Get all insights
GET    /api/insights/unread            # Get unread insights
GET    /api/insights/generate          # Generate monthly insights

PUT    /api/insights/{id}/read         # Mark insight as read
PUT    /api/insights/{id}/action-taken # Mark action taken

GET    /api/insights/spending-patterns # Analyze spending patterns
GET    /api/insights/savings-opportunities # Find savings opportunities
GET    /api/insights/budget-performance # Analyze budget performance
GET    /api/insights/loan-optimization/{loanId} # Loan optimization suggestions
```

### Income & Expenses (Existing)
```
POST   /api/income                     # Record income
GET    /api/income                     # Get income records
PUT    /api/income/{id}                # Update income
DELETE /api/income/{id}                # Delete income

POST   /api/spending                   # Record expense
GET    /api/spending                   # Get expense records
PUT    /api/spending/{id}              # Update expense
DELETE /api/spending/{id}              # Delete expense
```

### Loan Management (Enhanced)
```
POST   /api/loans                      # Create loan
GET    /api/loans                      # Get user loans
GET    /api/loans/{id}                 # Get loan details
PUT    /api/loans/{id}                 # Update loan
DELETE /api/loans/{id}                 # Delete loan

POST   /api/loans/{id}/payments        # Record payment
GET    /api/loans/{id}/payments        # Get payment history
GET    /api/loans/{id}/balance         # Get remaining balance
GET    /api/loans/{id}/early-payoff    # Calculate early payoff savings
```

## 🧠 AI-Powered Insights

The system provides intelligent financial insights:

### Spending Analysis
- **Pattern Detection**: Identify unusual spending patterns
- **Category Analysis**: Compare spending across categories and time periods
- **Merchant Analysis**: Track spending with specific merchants
- **Trend Analysis**: Identify increasing/decreasing spending trends

### Savings Opportunities
- **Recurring Expense Optimization**: Suggest reviewing recurring subscriptions
- **Budget Reallocation**: Identify underutilized budget categories
- **Spending Reduction**: Highlight categories with potential for savings
- **Goal Achievement**: Suggest ways to reach savings goals faster

### Loan Optimization
- **Extra Payment Benefits**: Calculate interest savings from extra payments
- **Refinancing Opportunities**: Identify potential refinancing benefits
- **Payment Strategy**: Suggest optimal payment strategies
- **Early Payoff Planning**: Create plans for early loan payoff

## 🚀 Development Plan

### Phase 1: Foundation Enhancement ✅
- [x] Enhanced domain models with new entities
- [x] Budget management system
- [x] Savings goals tracking
- [x] Financial insights framework
- [x] Loan payment tracking

### Phase 2: Service Implementation (In Progress)
- [x] Budget service with CRUD operations
- [x] Financial insight service with AI analysis
- [x] Enhanced loan service with early payoff calculations
- [ ] Savings goal service implementation
- [ ] Enhanced income/expense services

### Phase 3: API Enhancement
- [x] Budget management endpoints
- [x] Financial insights endpoints
- [ ] Savings goal endpoints
- [ ] Enhanced loan endpoints
- [ ] Analytics and reporting endpoints

### Phase 4: Advanced Features
- [ ] Automated insight generation (background service)
- [ ] Email/push notifications for insights
- [ ] Advanced analytics and reporting
- [ ] Data export functionality
- [ ] Integration with bank APIs

### Phase 5: Testing & Documentation
- [ ] Comprehensive unit tests
- [ ] Integration tests
- [ ] API documentation (Swagger/OpenAPI)
- [ ] Performance optimization
- [ ] Security enhancements

## 🛠️ Technology Stack

- **.NET 8**: Latest .NET framework
- **Entity Framework Core**: ORM for data access
- **SQL Server**: Primary database
- **Minimal APIs**: Lightweight API endpoints
- **Clean Architecture**: Separation of concerns
- **Domain Driven Design**: Business-focused design
- **Unit of Work Pattern**: Data consistency
- **Repository Pattern**: Data abstraction

## 🔒 Security Features

- User-based data isolation
- JWT authentication
- Authorization on all endpoints
- Input validation
- SQL injection prevention
- XSS protection

## 📈 Performance Considerations

- Efficient database queries with proper indexing
- Lazy loading for navigation properties
- Pagination for large data sets
- Caching for frequently accessed data
- Background processing for heavy computations

## 🧪 Testing Strategy

- **Unit Tests**: Domain logic and services
- **Integration Tests**: API endpoints and database
- **Performance Tests**: Load testing for critical paths
- **Security Tests**: Authentication and authorization

## 📝 Next Steps

1. **Complete Service Implementation**: Finish implementing all application services
2. **Add Missing Endpoints**: Complete API endpoint coverage
3. **Implement Background Jobs**: Automated insight generation
4. **Add Comprehensive Testing**: Unit and integration tests
5. **Performance Optimization**: Database indexing and query optimization
6. **Security Hardening**: Additional security measures
7. **Documentation**: Complete API documentation and user guides

## 🤝 Contributing

This project follows Clean Code principles and Domain Driven Design. When contributing:

1. Follow the established architecture patterns
2. Write comprehensive tests
3. Use meaningful names and clear code structure
4. Document complex business logic
5. Ensure proper error handling and validation
