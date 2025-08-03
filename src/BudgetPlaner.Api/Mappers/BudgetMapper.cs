using BudgetPlaner.Contracts.Api.Budget;
using BudgetPlaner.Domain;
using Sqids;
using BudgetPeriodType = BudgetPlaner.Domain.BudgetPeriodType;
using BudgetStatus = BudgetPlaner.Domain.BudgetStatus;

namespace BudgetPlaner.Api.Mappers;

public static class BudgetMapper
{
    public static BudgetEntity MapToEntity(this BudgetRequest request, SqidsEncoder<int> sqidsEncoder)
    {
        return new BudgetEntity
        {
            Name = request.Name,
            CurrencyId = sqidsEncoder.Decode(request.CurrencyId).SingleOrDefault(),
            EndDate = request.EndDate?.ToUniversalTime(),
            StartDate = request.StartDate?.ToUniversalTime(),
            PeriodType = (BudgetPeriodType)request.PeriodType,
            Status = (BudgetStatus)request.Status,
            TotalBudgetAmount = request.TotalBudgetAmount,
            UserId = "",
        };
    }
    
    public static IEnumerable<BudgetResponse> MapToResponse(this IEnumerable<BudgetEntity> entity,
        SqidsEncoder<int> sqids)
    {
        return entity.Select(x => new BudgetResponse
        {
            Id = sqids.Encode(x.Id),
            Name = x.Name,
            EndDate = x.EndDate,
            PeriodType = (Contracts.Api.Budget.BudgetPeriodType)x.PeriodType,
            Status = (Contracts.Api.Budget.BudgetStatus)x.Status,
            TotalBudgetAmount = x.TotalBudgetAmount,
            CurrencyName = $"{x.Currency?.Name} ({x.Currency?.Code})"
        });
    }
}