using BudgetPlaner.Api.Attributes;

namespace BudgetPlaner.Api.Services.Credit;

[ScopedRegistration]
public interface ILoanService
{
    Task GenerateCreditInterestRates(int loanId, string userId);
}