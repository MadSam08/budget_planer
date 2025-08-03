using BudgetPlaner.Contracts.Api;
using BudgetPlaner.Contracts.Api.Loan;
using BudgetPlaner.Domain;
using Sqids;

namespace BudgetPlaner.Api.Mappers;

public static class IncomeMapper
{
    public static IncomeEntity MapToEntity(this IncomeRequest request)
    {
        return new IncomeEntity
        {
            Description = request.Description,
            UserId = "",
            CurrencyId = request.CurrencyId,
            Value = request.Value,
            CategoryId = request.CategoryId,
            ActualDateOfIncome = request.ActualDateOfIncome
        };
    }

    public static IncomeRequest MapToModel(this IncomeEntity entity, SqidsEncoder<int> sqids)
    {
        return new IncomeRequest
        {
            Id = sqids.Encode(entity.Id),
            Description = entity.Description,
            CurrencyId = entity.CurrencyId,
            Value = entity.Value,
            CategoryId = entity.CategoryId,
            ActualDateOfIncome = entity.ActualDateOfIncome,
            CategoryName = entity.Category?.Name,
            CurrencyName = entity.Currency?.Name
        };
    }

    public static IEnumerable<IncomeRequest> MapToModel(this IEnumerable<IncomeEntity> entity,
        SqidsEncoder<int> sqids)
    {
        return entity.Select(x => new IncomeRequest
        {
            Id = sqids.Encode(x.Id),
            Description = x.Description,
            CurrencyId = x.CurrencyId,
            Value = x.Value,
            CategoryId = x.CategoryId,
            ActualDateOfIncome = x.ActualDateOfIncome,
            CategoryName = x.Category?.Name,
            CurrencyName = x.Currency?.Name
        });
    }
}