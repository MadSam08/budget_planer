using BudgetPlaner.Sdk.Constants;
using BudgetPlaner.Contracts.Api.Loan;
using Refit;

namespace BudgetPlaner.Sdk.Interfaces;

[Headers("Authorization: Bearer","Content-Type: application/json; charset=UTF-8", "Accept: application/json")]
public interface IExpensesApi
{
    [Get(ApiEndpoints.Expenses.GetAll)]
    Task<IEnumerable<SpendingRequest>> GetExpensesAsync();

    [Get(ApiEndpoints.Expenses.Get)]
    Task<SpendingRequest> GetExpenseAsync(string id);

    [Post(ApiEndpoints.Expenses.Create)]
    Task<SpendingRequest> CreateExpenseAsync([Body] SpendingRequest request);

    [Put(ApiEndpoints.Expenses.Update)]
    Task<SpendingRequest> UpdateExpenseAsync(string id, [Body] SpendingRequest request);

    [Delete(ApiEndpoints.Expenses.Delete)]
    Task DeleteExpenseAsync(string id);
} 