using BudgetPlaner.Domain;

namespace BudgetPlaner.Application.Services.Budget;

public class BudgetService(IUnitOfWork<BudgetPlanerContext> unitOfWork) : IBudgetService
{
    public async Task<BudgetEntity> CreateBudgetAsync(BudgetEntity budget, string userId)
    {
        budget.UserId = userId;
        budget.CreateDate = DateTime.UtcNow;
        budget.UpdateDate = DateTime.UtcNow;
        
        await unitOfWork.Repository<BudgetEntity>().AddAsync(budget);
        await unitOfWork.Complete();
        
        return budget;
    }

    public async Task<BudgetEntity?> GetBudgetByIdAsync(int budgetId, string userId)
    {
        return await unitOfWork.Repository<BudgetEntity>()
            .Where(b => b.Id == budgetId && b.UserId == userId)
            .Include(b => b.BudgetCategories!)
                .ThenInclude(bc => bc.Category)
            .Include(b => b.Currency)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<BudgetEntity>> GetUserBudgetsAsync(string userId)
    {
        return await unitOfWork.Repository<BudgetEntity>()
            .Where(b => b.UserId == userId)
            .Include(b => b.Currency)
            .OrderByDescending(b => b.CreateDate)
            .ToListAsync();
    }

    public async Task<BudgetEntity> UpdateBudgetAsync(BudgetEntity budget, string userId)
    {
        var existingBudget = await unitOfWork.Repository<BudgetEntity>()
            .Where(b => b.Id == budget.Id && b.UserId == userId)
            .FirstOrDefaultAsync();

        if (existingBudget == null)
            throw new InvalidOperationException("Budget not found");

        existingBudget.Name = budget.Name;
        existingBudget.TotalBudgetAmount = budget.TotalBudgetAmount;
        existingBudget.PeriodType = budget.PeriodType;
        existingBudget.StartDate = budget.StartDate;
        existingBudget.EndDate = budget.EndDate;
        existingBudget.Status = budget.Status;
        existingBudget.UpdateDate = DateTime.UtcNow;

        await unitOfWork.Complete();
        return existingBudget;
    }

    public async Task<bool> DeleteBudgetAsync(int budgetId, string userId)
    {
        var budget = await unitOfWork.Repository<BudgetEntity>()
            .Where(b => b.Id == budgetId && b.UserId == userId)
            .FirstOrDefaultAsync();

        if (budget == null) return false;

        unitOfWork.Repository<BudgetEntity>().Remove(budget);
        await unitOfWork.Complete();
        return true;
    }

    public async Task<BudgetCategoryEntity> AddCategoryToBudgetAsync(int budgetId, int categoryId, decimal allocatedAmount, string userId)
    {
        var budget = await GetBudgetByIdAsync(budgetId, userId);
        if (budget == null)
            throw new InvalidOperationException("Budget not found");

        var budgetCategory = new BudgetCategoryEntity
        {
            BudgetId = budgetId,
            CategoryId = categoryId,
            AllocatedAmount = allocatedAmount,
            SpentAmount = 0,
            UserId = userId,
            CreateDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow
        };

        await unitOfWork.Repository<BudgetCategoryEntity>().AddAsync(budgetCategory);
        await unitOfWork.Complete();

        return budgetCategory;
    }

    public async Task<BudgetCategoryEntity> UpdateBudgetCategoryAsync(int budgetCategoryId, decimal allocatedAmount, string userId)
    {
        var budgetCategory = await unitOfWork.Repository<BudgetCategoryEntity>()
            .Where(bc => bc.Id == budgetCategoryId && bc.UserId == userId)
            .FirstOrDefaultAsync();

        if (budgetCategory == null)
            throw new InvalidOperationException("Budget category not found");

        budgetCategory.AllocatedAmount = allocatedAmount;
        budgetCategory.UpdateDate = DateTime.UtcNow;

        await unitOfWork.Complete();
        return budgetCategory;
    }

    public async Task<bool> RemoveCategoryFromBudgetAsync(int budgetCategoryId, string userId)
    {
        var budgetCategory = await unitOfWork.Repository<BudgetCategoryEntity>()
            .Where(bc => bc.Id == budgetCategoryId && bc.UserId == userId)
            .FirstOrDefaultAsync();

        if (budgetCategory == null) return false;

        unitOfWork.Repository<BudgetCategoryEntity>().Remove(budgetCategory);
        await unitOfWork.Complete();
        return true;
    }

    public async Task<decimal> GetBudgetUtilizationAsync(int budgetId, string userId)
    {
        var budget = await GetBudgetByIdAsync(budgetId, userId);
        if (budget?.BudgetCategories == null) return 0;

        var totalSpent = budget.BudgetCategories.Sum(bc => bc.SpentAmount);
        return budget.TotalBudgetAmount > 0 ? (totalSpent / budget.TotalBudgetAmount) * 100 : 0;
    }

    public async Task<IEnumerable<BudgetCategoryEntity>> GetOverBudgetCategoriesAsync(int budgetId, string userId)
    {
        return await unitOfWork.Repository<BudgetCategoryEntity>()
            .Where(bc => bc.BudgetId == budgetId && bc.UserId == userId && bc.SpentAmount > bc.AllocatedAmount)
            .Include(bc => bc.Category)
            .ToListAsync();
    }
} 