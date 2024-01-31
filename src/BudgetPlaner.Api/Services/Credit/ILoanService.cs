using BudgetPlaner.Api.Attributes;
using BudgetPlaner.Models.ApiResponse;

namespace BudgetPlaner.Api.Services.Credit;

[ScopedRegistration]
public interface ILoanService
{
    Task GenerateCreditInterestRates(int loanId, string userId);
}