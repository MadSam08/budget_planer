namespace BudgetPlaner.Application.Services.Credit;

public interface ILoanService
{
    Task GenerateCreditInterestRates(int loanId, string userId);
}