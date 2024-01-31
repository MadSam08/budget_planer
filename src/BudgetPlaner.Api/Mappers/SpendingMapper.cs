using BudgetPlaner.Models.ApiResponse;
using BudgetPlaner.Models.Domain;
using Sqids;

namespace BudgetPlaner.Api.Mappers;

public static class SpendingMapper
{
    public static SpendingEntity MapToEntity(this SpendingModel model)
    {
        return new SpendingEntity
        {
            Description = model.Description,
            UserId = "",
            CurrencyId = model.CurrencyId,
            Value = model.Value,
            CategoryId = model.CategoryId,
            ActualDateOfSpending = model.ActualDateOfSpending
        };
    }

    public static SpendingModel MapToModel(this SpendingEntity entity, SqidsEncoder<int> sqids)
    {
        return new SpendingModel
        {
            Id = sqids.Encode(entity.Id),
            Description = entity.Description,
            CurrencyId = entity.CurrencyId,
            Value = entity.Value,
            CategoryId = entity.CategoryId,
            ActualDateOfSpending = entity.ActualDateOfSpending,
            CategoryName = entity.Category?.Name,
            CurrencyName = entity.Currency?.Name
        };
    }

    public static IEnumerable<SpendingModel> MapToModel(this IEnumerable<SpendingEntity> entity,
        SqidsEncoder<int> sqids)
    {
        return entity.Select(x => new SpendingModel
        {
            Id = sqids.Encode(x.Id),
            Description = x.Description,
            CurrencyId = x.CurrencyId,
            Value = x.Value,
            CategoryId = x.CategoryId,
            ActualDateOfSpending = x.ActualDateOfSpending,
            CategoryName = x.Category?.Name,
            CurrencyName = x.Currency?.Name
        });
    }
}