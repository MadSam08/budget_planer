using BudgetPlaner.Domain;

namespace BudgetPlaner.Application.Services.Credit;

public interface ILoanService
{
    Task GenerateCreditInterestRates(int loanId, string userId);
    Task<LoanEntity?> GetLoanByIdAsync(int loanId, string userId);
    Task<IEnumerable<LoanEntity>> GetUserLoansAsync(string userId);
    Task<LoanPaymentEntity> RecordPaymentAsync(int loanId, decimal amount, PaymentType type, string? notes, string userId);
    Task<decimal> CalculateEarlyPayoffSavingsAsync(int loanId, decimal extraMonthlyPayment, string userId);
    Task<int> CalculateMonthsToPayoffAsync(int loanId, decimal extraMonthlyPayment, string userId);
    Task<IEnumerable<LoanPaymentEntity>> GetLoanPaymentsAsync(int loanId, string userId);
    Task<decimal> GetRemainingBalanceAsync(int loanId, string userId);
    Task UpdateLoanBalanceAsync(int loanId, string userId);
}