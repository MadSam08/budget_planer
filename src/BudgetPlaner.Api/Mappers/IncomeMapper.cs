using BudgetPlaner.Contracts.Api;
using BudgetPlaner.Contracts.Api.Loan;
using BudgetPlaner.Domain;
using Sqids;

namespace BudgetPlaner.Api.Mappers;

public static class IncomeMapper
{
    public static IncomeEntity MapToEntity(this IncomeModel model)
    {
        return new IncomeEntity
        {
            Description = model.Description,
            UserId = "",
            CurrencyId = model.CurrencyId,
            Value = model.Value,
            CategoryId = model.CategoryId,
            ActualDateOfIncome = model.ActualDateOfIncome
        };
    }

    public static IncomeModel MapToModel(this IncomeEntity entity, SqidsEncoder<int> sqids)
    {
        return new IncomeModel
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

    public static IEnumerable<IncomeModel> MapToModel(this IEnumerable<IncomeEntity> entity,
        SqidsEncoder<int> sqids)
    {
        return entity.Select(x => new IncomeModel
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