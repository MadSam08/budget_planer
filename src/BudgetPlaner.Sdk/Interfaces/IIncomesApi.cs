using BudgetPlaner.Sdk.Constants;
using BudgetPlaner.Contracts.Api.Loan;
using Refit;

namespace BudgetPlaner.Sdk.Interfaces;

[Headers("Authorization: Bearer")]
public interface IIncomesApi
{
    [Get(ApiEndpoints.Incomes.GetAll)]
    Task<IEnumerable<IncomeModel>> GetIncomesAsync();

    [Get(ApiEndpoints.Incomes.Get)]
    Task<IncomeModel> GetIncomeAsync(string id);

    [Post(ApiEndpoints.Incomes.Create)]
    Task<IncomeModel> CreateIncomeAsync([Body] IncomeModel request);

    [Put(ApiEndpoints.Incomes.Update)]
    Task<IncomeModel> UpdateIncomeAsync(string id, [Body] IncomeModel request);

    [Delete(ApiEndpoints.Incomes.Delete)]
    Task DeleteIncomeAsync(string id);
} 