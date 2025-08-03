using BudgetPlaner.Sdk.Constants;
using BudgetPlaner.Contracts.Api.Loan;
using Refit;

namespace BudgetPlaner.Sdk.Interfaces;

[Headers("Authorization: Bearer","Content-Type: application/json; charset=UTF-8", "Accept: application/json")]
public interface IIncomesApi
{
    [Get(ApiEndpoints.Incomes.GetAll)]
    Task<IEnumerable<IncomeRequest>> GetIncomesAsync();

    [Get(ApiEndpoints.Incomes.Get)]
    Task<IncomeRequest> GetIncomeAsync(string id);

    [Post(ApiEndpoints.Incomes.Create)]
    Task<IncomeRequest> CreateIncomeAsync([Body] IncomeRequest request);

    [Put(ApiEndpoints.Incomes.Update)]
    Task<IncomeRequest> UpdateIncomeAsync(string id, [Body] IncomeRequest request);

    [Delete(ApiEndpoints.Incomes.Delete)]
    Task DeleteIncomeAsync(string id);
} 