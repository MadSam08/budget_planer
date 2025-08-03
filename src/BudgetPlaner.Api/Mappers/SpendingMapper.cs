using BudgetPlaner.Contracts.Api;
using BudgetPlaner.Contracts.Api.Loan;
using BudgetPlaner.Domain;
using Sqids;

namespace BudgetPlaner.Api.Mappers;

public static class SpendingMapper
{
    public static SpendingEntity MapToEntity(this SpendingRequest request, SqidsEncoder<int> sqids)
    {
        return new SpendingEntity
        {
            Description = request.Description,
            UserId = "",
            CurrencyId = sqids.Decode(request.CurrencyId).SingleOrDefault(),
            Value = request.Value,
            CategoryId = sqids.Decode(request.CategoryId).SingleOrDefault(),
            ActualDateOfSpending = request.ActualDateOfSpending
        };
    }

    public static SpendingRequest MapToModel(this SpendingEntity entity, SqidsEncoder<int> sqids)
    {
        return new SpendingRequest
        {
            Id = sqids.Encode(entity.Id),
            Description = entity.Description,
            CurrencyId = sqids.Encode(entity.CurrencyId),
            Value = entity.Value,
            CategoryId = sqids.Encode(entity.CategoryId),
            ActualDateOfSpending = entity.ActualDateOfSpending,
            CategoryName = entity.Category?.Name,
            CurrencyName = entity.Currency?.Name
        };
    }

    public static IEnumerable<SpendingRequest> MapToModel(this IEnumerable<SpendingEntity> entity,
        SqidsEncoder<int> sqids)
    {
        return entity.Select(x => new SpendingRequest
        {
            Id = sqids.Encode(x.Id),
            Description = x.Description,
            CurrencyId = sqids.Encode(x.CurrencyId),
            Value = x.Value,
            CategoryId = sqids.Encode(x.CategoryId),
            ActualDateOfSpending = x.ActualDateOfSpending,
            CategoryName = x.Category?.Name,
            CurrencyName = x.Currency?.Name
        });
    }
}