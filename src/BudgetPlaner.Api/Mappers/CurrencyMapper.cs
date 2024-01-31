using BudgetPlaner.Models.ApiResponse;
using BudgetPlaner.Models.Domain;
using Sqids;

namespace BudgetPlaner.Api.Mappers;

public static class CurrencyMapper
{
    public static CurrencyEntity MapToEntity(this CurrencyModel model)
    {
        return new CurrencyEntity
        {
            Name = model.Name,
            UserId = "",
            Code = model.Code,
            NationalBankRate = model.NationalBankRate
        };
    }

    public static CurrencyModel MapToModel(this CurrencyEntity entity, SqidsEncoder<int> sqids)
    {
        return new CurrencyModel
        {
            Id = sqids.Encode(entity.Id),
            Name = entity.Name,
            Code = entity.Code,
            NationalBankRate = entity.NationalBankRate
        };
    }

    public static IEnumerable<CurrencyModel> MapToModel(this IEnumerable<CurrencyEntity> entity,
        SqidsEncoder<int> sqids)
    {
        return entity.Select(x => new CurrencyModel
        {
            Id = sqids.Encode(x.Id),
            Name = x.Name,
            Code = x.Code,
            NationalBankRate = x.NationalBankRate
        });
    }
}