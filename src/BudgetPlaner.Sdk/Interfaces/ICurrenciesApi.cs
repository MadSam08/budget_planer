using BudgetPlaner.Contracts.Api;
using BudgetPlaner.Sdk.Constants;
using Refit;

namespace BudgetPlaner.Sdk.Interfaces;

[Headers("Authorization: Bearer")]
public interface ICurrenciesApi
{
    [Get(ApiEndpoints.Currencies.GetAll)]
    Task<IEnumerable<CurrencyModel>> GetCurrenciesAsync();

    [Get(ApiEndpoints.Currencies.Get)]
    Task<CurrencyModel> GetCurrencyAsync(string id);

    [Post(ApiEndpoints.Currencies.Create)]
    Task<IApiResponse> CreateCurrencyAsync([Body] CurrencyModel request);

    [Put(ApiEndpoints.Currencies.Update)]
    Task<IApiResponse> UpdateCurrencyAsync(string id, [Body] CurrencyModel request);

    [Delete(ApiEndpoints.Currencies.Delete)]
    Task DeleteCurrencyAsync(string id);

    [Patch(ApiEndpoints.Currencies.Restore)]
    Task RestoreCurrencyAsync(string id);
} 