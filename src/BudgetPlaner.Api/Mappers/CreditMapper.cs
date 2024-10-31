using BudgetPlaner.Models.Api;
using BudgetPlaner.Models.Domain;
using Sqids;

namespace BudgetPlaner.Api.Mappers;

public static class CreditMapper
{
    public static LoanEntity MapToEntity(this LoanModel model)
    {
        return new LoanEntity
        {
            Name = model.Name,
            UserId = "",
            CurrencyId = model.CurrencyId,
            Interest = model.Interest,
            Period = model.Period,
            Principal = model.Principal,
            AnnualRate = model.AnnualRate,
            BankName = model.BankName,
            TotalAmount = model.TotalValue,
            APR = model.APR,
            CreditStatus = model.CreditStatus
        };
    }

    public static LoanModel MapToModel(this LoanEntity entity, SqidsEncoder<int> sqids)
    {
        return new LoanModel
        {
            Id = sqids.Encode(entity.Id),
            Name = entity.Name,
            CurrencyId = entity.CurrencyId,
            Interest = entity.Interest,
            Period = entity.Period,
            Principal = entity.Principal,
            AnnualRate = entity.AnnualRate,
            BankName = entity.BankName,
            TotalValue = entity.TotalAmount,
            APR = entity.APR,
            CurrencyName = entity.Currency?.Name,
            CreditStatus = entity.CreditStatus
        };
    }

    public static IEnumerable<LoanModel> MapToModel(this IEnumerable<LoanEntity> entity,
        SqidsEncoder<int> sqids)
    {
        return entity.Select(x => new LoanModel
        {
            Id = sqids.Encode(x.Id),
            Name = x.Name,
            CurrencyId = x.CurrencyId,
            Interest = x.Interest,
            Period = x.Period,
            Principal = x.Principal,
            AnnualRate = x.AnnualRate,
            BankName = x.BankName,
            TotalValue = x.TotalAmount,
            APR = x.APR,
            CurrencyName = x.Currency?.Name,
            CreditStatus = x.CreditStatus
        });
    }
}