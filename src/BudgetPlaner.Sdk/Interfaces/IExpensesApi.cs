using BudgetPlaner.Sdk.Constants;
using BudgetPlaner.Contracts.Api.Loan;
using Refit;

namespace BudgetPlaner.Sdk.Interfaces;

[Headers("Authorization: Bearer")]
public interface IExpensesApi
{
    [Get(ApiEndpoints.Expenses.GetAll)]
    Task<IEnumerable<SpendingModel>> GetExpensesAsync();

    [Get(ApiEndpoints.Expenses.Get)]
    Task<SpendingModel> GetExpenseAsync(string id);

    [Post(ApiEndpoints.Expenses.Create)]
    Task<SpendingModel> CreateExpenseAsync([Body] SpendingModel request);

    [Put(ApiEndpoints.Expenses.Update)]
    Task<SpendingModel> UpdateExpenseAsync(string id, [Body] SpendingModel request);

    [Delete(ApiEndpoints.Expenses.Delete)]
    Task DeleteExpenseAsync(string id);
} 