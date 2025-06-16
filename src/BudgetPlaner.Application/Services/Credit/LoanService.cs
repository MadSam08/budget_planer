using BudgetPlaner.Domain;
using BudgetPlaner.Infrastructure.DatabaseContext;
using BudgetPlaner.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace BudgetPlaner.Application.Services.Credit;

public class LoanService(IUnitOfWork<BudgetPlanerContext> unitOfWork) : ILoanService
{
    public async Task GenerateCreditInterestRates(int loanId, string userId)
    {
        var loan = await GetLoanByIdAsync(loanId, userId);
        if (loan == null) return;

        var rates = new List<LoanInterestRateEntity>();
        var monthlyRate = loan.AnnualRate / 100 / 12;
        var remainingBalance = loan.Principal;

        for (int month = 1; month <= loan.Period; month++)
        {
            var interestPayment = remainingBalance * monthlyRate;
            var principalPayment = loan.MonthlyPayment - interestPayment;
            remainingBalance -= principalPayment;

            rates.Add(new LoanInterestRateEntity
            {
                LoanId = loanId,
                Month = month,
                InterestPayType = 1, // Regular payment type
                InterestValue = interestPayment,
                PrincipalValue = principalPayment,
                UserId = userId,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow
            });

            if (remainingBalance <= 0) break;
        }

        await unitOfWork.Repository<LoanInterestRateEntity>().AddRangeAsync(rates);
        await unitOfWork.Complete();
    }

    public async Task<LoanEntity?> GetLoanByIdAsync(int loanId, string userId)
    {
        return await unitOfWork.Repository<LoanEntity>()
            .Where(l => l.Id == loanId && l.UserId == userId)
            .Include(l => l.Currency)
            .Include(l => l.ScheduledRates!)
            .Include(l => l.Payments!)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<LoanEntity>> GetUserLoansAsync(string userId)
    {
        return await unitOfWork.Repository<LoanEntity>()
            .Where(l => l.UserId == userId)
            .Include(l => l.Currency)
            .OrderByDescending(l => l.CreateDate)
            .ToListAsync();
    }

    public async Task<LoanPaymentEntity> RecordPaymentAsync(int loanId, decimal amount, PaymentType type, string? notes, string userId)
    {
        var loan = await GetLoanByIdAsync(loanId, userId);
        if (loan == null)
            throw new InvalidOperationException("Loan not found");

        // Calculate principal and interest portions
        var monthlyRate = loan.AnnualRate / 100 / 12;
        var interestAmount = loan.RemainingBalance * monthlyRate;
        var principalAmount = Math.Min(amount - interestAmount, loan.RemainingBalance);
        if (principalAmount < 0)
        {
            principalAmount = 0;
        }

        var payment = new LoanPaymentEntity
        {
            LoanId = loanId,
            Amount = amount,
            PrincipalAmount = principalAmount,
            InterestAmount = interestAmount,
            PaymentDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow, // This should be calculated based on loan schedule
            Status = PaymentStatus.Paid,
            Type = type,
            Notes = notes,
            UserId = userId,
            CreateDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow
        };

        await unitOfWork.Repository<LoanPaymentEntity>().AddAsync(payment);

        // Update loan balance and payments made
        loan.RemainingBalance -= principalAmount;
        loan.PaymentsMade++;
        loan.UpdateDate = DateTime.UtcNow;

        if (loan.RemainingBalance <= 0)
        {
            loan.Status = LoanStatus.PaidOff;
            loan.RemainingBalance = 0;
        }

        await unitOfWork.Complete();
        return payment;
    }

    public async Task<decimal> CalculateEarlyPayoffSavingsAsync(int loanId, decimal extraMonthlyPayment, string userId)
    {
        var loan = await GetLoanByIdAsync(loanId, userId);
        if (loan == null) return 0;

        var monthlyRate = loan.AnnualRate / 100 / 12;
        var currentBalance = loan.RemainingBalance;
        var regularPayment = loan.MonthlyPayment;
        var newPayment = regularPayment + extraMonthlyPayment;

        // Calculate total interest with regular payments
        var regularInterest = CalculateTotalInterest(currentBalance, regularPayment, monthlyRate);

        // Calculate total interest with extra payments
        var extraInterest = CalculateTotalInterest(currentBalance, newPayment, monthlyRate);

        return regularInterest - extraInterest;
    }

    public async Task<int> CalculateMonthsToPayoffAsync(int loanId, decimal extraMonthlyPayment, string userId)
    {
        var loan = await GetLoanByIdAsync(loanId, userId);
        if (loan == null) return 0;

        var monthlyRate = loan.AnnualRate / 100 / 12;
        var currentBalance = loan.RemainingBalance;
        var newPayment = loan.MonthlyPayment + extraMonthlyPayment;

        return CalculatePayoffMonths(currentBalance, newPayment, monthlyRate);
    }

    public async Task<IEnumerable<LoanPaymentEntity>> GetLoanPaymentsAsync(int loanId, string userId)
    {
        return await unitOfWork.Repository<LoanPaymentEntity>()
            .Where(lp => lp.LoanId == loanId && lp.UserId == userId)
            .OrderByDescending(lp => lp.PaymentDate)
            .ToListAsync();
    }

    public async Task<decimal> GetRemainingBalanceAsync(int loanId, string userId)
    {
        var loan = await GetLoanByIdAsync(loanId, userId);
        return loan?.RemainingBalance ?? 0;
    }

    public async Task UpdateLoanBalanceAsync(int loanId, string userId)
    {
        var loan = await GetLoanByIdAsync(loanId, userId);
        if (loan?.Payments == null) return;

        var totalPrincipalPaid = loan.Payments.Sum(p => p.PrincipalAmount);
        loan.RemainingBalance = loan.Principal - totalPrincipalPaid;
        loan.PaymentsMade = loan.Payments.Count(p => p.Status == PaymentStatus.Paid);
        loan.UpdateDate = DateTime.UtcNow;

        if (loan.RemainingBalance <= 0)
        {
            loan.Status = LoanStatus.PaidOff;
            loan.RemainingBalance = 0;
        }

        await unitOfWork.Complete();
    }

    private decimal CalculateTotalInterest(decimal balance, decimal monthlyPayment, decimal monthlyRate)
    {
        decimal totalInterest = 0;
        decimal currentBalance = balance;

        while (currentBalance > 0)
        {
            var interestPayment = currentBalance * monthlyRate;
            var principalPayment = monthlyPayment - interestPayment;

            if (principalPayment <= 0) break; // Payment doesn't cover interest

            totalInterest += interestPayment;
            currentBalance -= principalPayment;

            if (currentBalance < 0) currentBalance = 0;
        }

        return totalInterest;
    }

    private int CalculatePayoffMonths(decimal balance, decimal monthlyPayment, decimal monthlyRate)
    {
        int months = 0;
        decimal currentBalance = balance;

        while (currentBalance > 0 && months < 1000) // Safety limit
        {
            var interestPayment = currentBalance * monthlyRate;
            var principalPayment = monthlyPayment - interestPayment;

            if (principalPayment <= 0) break; // Payment doesn't cover interest

            currentBalance -= principalPayment;
            months++;

            if (currentBalance < 0) currentBalance = 0;
        }

        return months;
    }
}