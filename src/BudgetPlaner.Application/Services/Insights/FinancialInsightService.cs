using BudgetPlaner.Domain;
using BudgetPlaner.Infrastructure.DatabaseContext;
using BudgetPlaner.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace BudgetPlaner.Application.Services.Insights;

public class FinancialInsightService(IUnitOfWork<BudgetPlanerContext> unitOfWork) : IFinancialInsightService
{
    public async Task<IEnumerable<FinancialInsightEntity>> GenerateMonthlyInsightsAsync(string userId)
    {
        var insights = new List<FinancialInsightEntity>();
        
        // Combine all analysis types
        insights.AddRange(await AnalyzeSpendingPatternsAsync(userId));
        insights.AddRange(await GenerateSavingsOpportunitiesAsync(userId));
        insights.AddRange(await AnalyzeBudgetPerformanceAsync(userId));
        
        // Save insights to database
        foreach (var insight in insights)
        {
            await CreateInsightAsync(insight, userId);
        }
        
        return insights;
    }

    public async Task<IEnumerable<FinancialInsightEntity>> GetUserInsightsAsync(string userId, bool unreadOnly = false)
    {
        var query = unitOfWork.Repository<FinancialInsightEntity>()
            .Where(i => i.UserId == userId && i.ValidUntil > DateTime.UtcNow);
            
        if (unreadOnly)
        {
            query = query.Where(i => !i.IsRead);
        }
        
        return await query
            .Include(i => i.Category)
            .OrderByDescending(i => i.Priority)
            .ThenByDescending(i => i.CreateDate)
            .ToListAsync();
    }

    public async Task<FinancialInsightEntity> CreateInsightAsync(FinancialInsightEntity insight, string userId)
    {
        insight.UserId = userId;
        insight.CreateDate = DateTime.UtcNow;
        insight.UpdateDate = DateTime.UtcNow;
        insight.IsRead = false;
        insight.IsActionTaken = false;
        
        if (insight.ValidUntil == default)
        {
            insight.ValidUntil = DateTime.UtcNow.AddDays(30); // Default 30 days validity
        }
        
        await unitOfWork.Repository<FinancialInsightEntity>().AddAsync(insight);
        await unitOfWork.Complete();
        
        return insight;
    }

    public async Task<bool> MarkInsightAsReadAsync(int insightId, string userId)
    {
        var insight = await unitOfWork.Repository<FinancialInsightEntity>()
            .Where(i => i.Id == insightId && i.UserId == userId)
            .FirstOrDefaultAsync();
            
        if (insight == null) return false;
        
        insight.IsRead = true;
        insight.UpdateDate = DateTime.UtcNow;
        
        await unitOfWork.Complete();
        return true;
    }

    public async Task<bool> MarkInsightActionTakenAsync(int insightId, string userId)
    {
        var insight = await unitOfWork.Repository<FinancialInsightEntity>()
            .Where(i => i.Id == insightId && i.UserId == userId)
            .FirstOrDefaultAsync();
            
        if (insight == null) return false;
        
        insight.IsActionTaken = true;
        insight.UpdateDate = DateTime.UtcNow;
        
        await unitOfWork.Complete();
        return true;
    }

    public async Task<IEnumerable<FinancialInsightEntity>> AnalyzeSpendingPatternsAsync(string userId)
    {
        var insights = new List<FinancialInsightEntity>();
        var currentMonth = DateTime.UtcNow.Month;
        var currentYear = DateTime.UtcNow.Year;
        
        // Get current month spending by category
        var currentMonthSpending = await unitOfWork.Repository<SpendingEntity>()
            .Where(s => s.UserId == userId && 
                       s.ActualDateOfSpending.Month == currentMonth && 
                       s.ActualDateOfSpending.Year == currentYear)
            .GroupBy(s => s.CategoryId)
            .Select(g => new { CategoryId = g.Key, TotalSpent = g.Sum(s => s.Value) })
            .ToListAsync();
            
        // Get previous month spending for comparison
        var previousMonth = currentMonth == 1 ? 12 : currentMonth - 1;
        var previousYear = currentMonth == 1 ? currentYear - 1 : currentYear;
        
        var previousMonthSpending = await unitOfWork.Repository<SpendingEntity>()
            .Where(s => s.UserId == userId && 
                       s.ActualDateOfSpending.Month == previousMonth && 
                       s.ActualDateOfSpending.Year == previousYear)
            .GroupBy(s => s.CategoryId)
            .Select(g => new { CategoryId = g.Key, TotalSpent = g.Sum(s => s.Value) })
            .ToListAsync();
            
        // Analyze spending increases
        foreach (var currentSpending in currentMonthSpending)
        {
            var previousSpending = previousMonthSpending
                .FirstOrDefault(p => p.CategoryId == currentSpending.CategoryId);
                
            if (previousSpending != null)
            {
                var increasePercentage = ((currentSpending.TotalSpent - previousSpending.TotalSpent) / previousSpending.TotalSpent) * 100;
                
                if (increasePercentage > 20) // 20% increase threshold
                {
                    insights.Add(new FinancialInsightEntity
                    {
                        Title = "Spending Increase Alert",
                        Message = $"Your spending in this category has increased by {increasePercentage:F1}% compared to last month.",
                        Type = InsightType.SpendingAlert,
                        Priority = increasePercentage > 50 ? InsightPriority.High : InsightPriority.Medium,
                        CategoryId = currentSpending.CategoryId,
                        ValidUntil = DateTime.UtcNow.AddDays(15),
                        UserId = userId
                    });
                }
            }
        }
        
        return insights;
    }

    public async Task<IEnumerable<FinancialInsightEntity>> GenerateSavingsOpportunitiesAsync(string userId)
    {
        var insights = new List<FinancialInsightEntity>();
        
        // Analyze recurring expenses that could be optimized
        var recurringExpenses = await unitOfWork.Repository<SpendingEntity>()
            .Where(s => s.UserId == userId && s.IsRecurring)
            .GroupBy(s => new { s.CategoryId, s.Merchant })
            .Where(g => g.Count() >= 3) // At least 3 occurrences
            .Select(g => new { 
                CategoryId = g.Key.CategoryId, 
                Merchant = g.Key.Merchant,
                AverageAmount = g.Average(s => s.Value),
                Count = g.Count()
            })
            .ToListAsync();
            
        foreach (var expense in recurringExpenses.Where(e => e.AverageAmount > 50)) // Focus on significant amounts
        {
            var potentialSavings = expense.AverageAmount * 0.1m; // Assume 10% potential savings
            
            insights.Add(new FinancialInsightEntity
            {
                Title = "Savings Opportunity",
                Message = $"Consider reviewing your recurring expense with {expense.Merchant}. You could potentially save ${potentialSavings:F2} per month.",
                Type = InsightType.SavingsOpportunity,
                Priority = InsightPriority.Medium,
                CategoryId = expense.CategoryId,
                PotentialSavings = potentialSavings,
                ValidUntil = DateTime.UtcNow.AddDays(30),
                UserId = userId
            });
        }
        
        return insights;
    }

    public async Task<IEnumerable<FinancialInsightEntity>> AnalyzeBudgetPerformanceAsync(string userId)
    {
        var insights = new List<FinancialInsightEntity>();
        
        // Get active budgets
        var activeBudgets = await unitOfWork.Repository<BudgetEntity>()
            .Where(b => b.UserId == userId && b.Status == BudgetStatus.Active)
            .Include(b => b.BudgetCategories!)
                .ThenInclude(bc => bc.Category)
            .ToListAsync();
            
        foreach (var budget in activeBudgets)
        {
            if (budget.BudgetCategories == null) continue;
            
            var overBudgetCategories = budget.BudgetCategories.Where(bc => bc.IsOverBudget).ToList();
            
            if (overBudgetCategories.Any())
            {
                foreach (var category in overBudgetCategories)
                {
                    var overspend = category.SpentAmount - category.AllocatedAmount;
                    
                    insights.Add(new FinancialInsightEntity
                    {
                        Title = "Budget Exceeded",
                        Message = $"You've exceeded your budget for {category.Category?.Name} by ${overspend:F2}.",
                        Type = InsightType.BudgetRecommendation,
                        Priority = InsightPriority.High,
                        CategoryId = category.CategoryId,
                        ValidUntil = DateTime.UtcNow.AddDays(7),
                        UserId = userId
                    });
                }
            }
            
            // Check for categories with low utilization (potential reallocation)
            var underutilizedCategories = budget.BudgetCategories
                .Where(bc => bc.AllocatedAmount > 0 && (bc.SpentAmount / bc.AllocatedAmount) < 0.5m)
                .ToList();
                
            foreach (var category in underutilizedCategories)
            {
                var unusedAmount = category.RemainingAmount;
                
                insights.Add(new FinancialInsightEntity
                {
                    Title = "Budget Reallocation Opportunity",
                    Message = $"You have ${unusedAmount:F2} unused in your {category.Category?.Name} budget. Consider reallocating to other categories.",
                    Type = InsightType.BudgetRecommendation,
                    Priority = InsightPriority.Low,
                    CategoryId = category.CategoryId,
                    PotentialSavings = unusedAmount,
                    ValidUntil = DateTime.UtcNow.AddDays(20),
                    UserId = userId
                });
            }
        }
        
        return insights;
    }

    public async Task<FinancialInsightEntity?> GenerateLoanOptimizationSuggestionAsync(int loanId, string userId)
    {
        var loan = await unitOfWork.Repository<LoanEntity>()
            .Where(l => l.Id == loanId && l.UserId == userId)
            .FirstOrDefaultAsync();
            
        if (loan == null) return null;
        
        // Calculate potential savings from extra payments
        var extraPaymentAmount = loan.MonthlyPayment * 0.1m; // 10% extra payment
        var interestSavings = CalculateInterestSavings(loan, extraPaymentAmount);
        
        if (interestSavings > 100) // Only suggest if savings are significant
        {
            return new FinancialInsightEntity
            {
                Title = "Loan Optimization Opportunity",
                Message = $"By paying an extra ${extraPaymentAmount:F2} monthly on your {loan.Name}, you could save ${interestSavings:F2} in interest and pay off the loan {CalculateMonthsSaved(loan, extraPaymentAmount)} months earlier.",
                Type = InsightType.LoanOptimization,
                Priority = InsightPriority.Medium,
                PotentialSavings = interestSavings,
                ValidUntil = DateTime.UtcNow.AddDays(45),
                UserId = userId
            };
        }
        
        return null;
    }
    
    private decimal CalculateInterestSavings(LoanEntity loan, decimal extraPayment)
    {
        // Simplified calculation - in real implementation, use proper amortization
        var monthsReduced = (int)(loan.RemainingBalance / extraPayment);
        return monthsReduced * (loan.MonthlyPayment * (loan.AnnualRate / 100 / 12));
    }
    
    private int CalculateMonthsSaved(LoanEntity loan, decimal extraPayment)
    {
        // Simplified calculation
        return (int)(loan.RemainingBalance / extraPayment);
    }
} 