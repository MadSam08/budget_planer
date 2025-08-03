using BudgetPlaner.Sdk.Constants;
using BudgetPlaner.Contracts.Api.Loan;
using Refit;

namespace BudgetPlaner.Sdk.Interfaces;

[Headers("Authorization: Bearer","Content-Type: application/json; charset=UTF-8", "Accept: application/json")]
public interface ILoansApi
{
    [Get(ApiEndpoints.Loans.GetAll)]
    Task<IEnumerable<LoanModel>> GetLoansAsync();

    [Get(ApiEndpoints.Loans.Get)]
    Task<LoanModel> GetLoanAsync(string id);

    [Post(ApiEndpoints.Loans.Create)]
    Task<LoanModel> CreateLoanAsync([Body] LoanModel request);

    [Put(ApiEndpoints.Loans.Update)]
    Task<LoanModel> UpdateLoanAsync(string id, [Body] LoanModel request);

    [Delete(ApiEndpoints.Loans.Delete)]
    Task DeleteLoanAsync(string id);

    [Get(ApiEndpoints.Loans.GetInterestRates)]
    Task<IEnumerable<LoanInterestRateModel>> GetInterestRatesAsync(string id);

    [Post(ApiEndpoints.Loans.GenerateInterestRates)]
    Task<IEnumerable<LoanInterestRateModel>> GenerateInterestRatesAsync(string id);
} 