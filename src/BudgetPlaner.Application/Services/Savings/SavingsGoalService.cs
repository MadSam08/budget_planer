using BudgetPlaner.Domain;
using BudgetPlaner.Infrastructure.DatabaseContext;
using BudgetPlaner.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace BudgetPlaner.Application.Services.Savings;

public class SavingsGoalService(IUnitOfWork<BudgetPlanerContext> unitOfWork) : ISavingsGoalService
{
    public async Task<SavingsGoalEntity> CreateSavingsGoalAsync(SavingsGoalEntity savingsGoal, string userId)
    {
        savingsGoal.UserId = userId;
        savingsGoal.CreateDate = DateTime.UtcNow;
        savingsGoal.UpdateDate = DateTime.UtcNow;
        
        await unitOfWork.Repository<SavingsGoalEntity>().AddAsync(savingsGoal);
        await unitOfWork.Complete();
        
        return savingsGoal;
    }

    public async Task<SavingsGoalEntity?> GetSavingsGoalByIdAsync(int goalId, string userId)
    {
        return await unitOfWork.Repository<SavingsGoalEntity>()
            .Where(sg => sg.Id == goalId && sg.UserId == userId)
            .Include(sg => sg.Currency)
            .Include(sg => sg.Contributions!)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<SavingsGoalEntity>> GetUserSavingsGoalsAsync(string userId)
    {
        return await unitOfWork.Repository<SavingsGoalEntity>()
            .Where(sg => sg.UserId == userId)
            .Include(sg => sg.Currency)
            .OrderByDescending(sg => sg.Priority)
            .ThenBy(sg => sg.TargetDate)
            .ToListAsync();
    }

    public async Task<SavingsGoalEntity> UpdateSavingsGoalAsync(SavingsGoalEntity savingsGoal, string userId)
    {
        var existingGoal = await unitOfWork.Repository<SavingsGoalEntity>()
            .Where(sg => sg.Id == savingsGoal.Id && sg.UserId == userId)
            .FirstOrDefaultAsync();

        if (existingGoal == null)
            throw new InvalidOperationException("Savings goal not found");

        existingGoal.Name = savingsGoal.Name;
        existingGoal.Description = savingsGoal.Description;
        existingGoal.TargetAmount = savingsGoal.TargetAmount;
        existingGoal.TargetDate = savingsGoal.TargetDate;
        existingGoal.Status = savingsGoal.Status;
        existingGoal.Priority = savingsGoal.Priority;
        existingGoal.UpdateDate = DateTime.UtcNow;

        await unitOfWork.Complete();
        return existingGoal;
    }

    public async Task<bool> DeleteSavingsGoalAsync(int goalId, string userId)
    {
        var deletedCount = await unitOfWork.Repository<SavingsGoalEntity>()
            .ExecuteDeleteAsync(sg => sg.Id == goalId && sg.UserId == userId);

        return deletedCount > 0;
    }

    public async Task<SavingsContributionEntity> AddContributionAsync(int goalId, decimal amount, ContributionType type, string? notes, string userId)
    {
        var goal = await GetSavingsGoalByIdAsync(goalId, userId);
        if (goal == null)
            throw new InvalidOperationException("Savings goal not found");

        var contribution = new SavingsContributionEntity
        {
            SavingsGoalId = goalId,
            Amount = amount,
            ContributionDate = DateTime.UtcNow,
            Type = type,
            Notes = notes,
            UserId = userId,
            CreateDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow
        };

        await unitOfWork.Repository<SavingsContributionEntity>().AddAsync(contribution);
        
        // Update the goal's current amount
        goal.CurrentAmount += amount;
        if (goal.CurrentAmount >= goal.TargetAmount)
        {
            goal.Status = SavingsGoalStatus.Completed;
        }
        goal.UpdateDate = DateTime.UtcNow;

        await unitOfWork.Complete();
        return contribution;
    }

    public async Task<IEnumerable<SavingsContributionEntity>> GetGoalContributionsAsync(int goalId, string userId)
    {
        return await unitOfWork.Repository<SavingsContributionEntity>()
            .Where(sc => sc.SavingsGoalId == goalId && sc.UserId == userId)
            .OrderByDescending(sc => sc.ContributionDate)
            .ToListAsync();
    }

    public async Task<decimal> CalculateRequiredMonthlySavingAsync(int goalId, string userId)
    {
        var goal = await GetSavingsGoalByIdAsync(goalId, userId);
        if (goal == null) return 0;

        return goal.RequiredMonthlySaving;
    }

    public async Task<IEnumerable<SavingsGoalEntity>> GetGoalsNearingDeadlineAsync(string userId, int daysThreshold = 30)
    {
        var thresholdDate = DateTime.UtcNow.AddDays(daysThreshold);
        
        return await unitOfWork.Repository<SavingsGoalEntity>()
            .Where(sg => sg.UserId == userId && 
                        sg.Status == SavingsGoalStatus.Active && 
                        sg.TargetDate <= thresholdDate &&
                        sg.CurrentAmount < sg.TargetAmount)
            .Include(sg => sg.Currency)
            .OrderBy(sg => sg.TargetDate)
            .ToListAsync();
    }

    public async Task<bool> MarkGoalAsCompletedAsync(int goalId, string userId)
    {
        var goal = await unitOfWork.Repository<SavingsGoalEntity>()
            .Where(sg => sg.Id == goalId && sg.UserId == userId)
            .FirstOrDefaultAsync();

        if (goal == null) return false;

        goal.Status = SavingsGoalStatus.Completed;
        goal.UpdateDate = DateTime.UtcNow;

        await unitOfWork.Complete();
        return true;
    }
} 