using BudgetPlaner.Api.Attributes;
using BudgetPlaner.Api.DatabaseContext;
using BudgetPlaner.Api.Repository.UnitOfWork;
using BudgetPlaner.Domain;
using Microsoft.EntityFrameworkCore;

namespace BudgetPlaner.Api.Services.Credit;

[ScopedRegistration]
public class LoanService(IUnitOfWork<BudgetPlanerContext> unitOfWork) : ILoanService
{
    public async Task GenerateCreditInterestRates(int loanId, string userId)
    {
        var loanEntity = await unitOfWork.Repository<LoanEntity>()
            .Where(x => x.Id == loanId && x.UserId.Equals(userId)).FirstOrDefaultAsync();

        if (loanEntity == null) return;

        // Convert interest rate into a decimal
        // eg. 6.5% = 0.065
        var annualRate = loanEntity.AnnualRate /= 100.0M;

        // Monthly interest rate 
        // is the yearly rate divided by 12
        var monthlyRate = annualRate / 12.0M;

        // Calculate the monthly payment
        var monthlyPayment =
            (loanEntity.TotalAmount * monthlyRate) /
            (decimal)(1 - Math.Pow((double)(1 + monthlyRate), -loanEntity.Period));

        var balance = loanEntity.TotalAmount ;

        List<LoanInterestRateEntity> schedule = [];
        for (var count = 0; count < loanEntity.Period; count++)
        {
            var interestPayment = balance * monthlyRate;
            var principalPayment = monthlyPayment - interestPayment;
            balance -= principalPayment;

            schedule.Add(new LoanInterestRateEntity
            {
                LoanId = loanId,
                UserId = userId,
                Month = count + 1,
                InterestValue = interestPayment,
                PrincipalValue = principalPayment,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
                InterestPayType = InterestPayType.Regular
            });
        }

        await unitOfWork.Repository<LoanInterestRateEntity>().AddRangeAsync(schedule);
        await unitOfWork.Complete();
    }
}