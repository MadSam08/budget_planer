namespace BudgetPlaner.Infrastructure.UnitOfWork;

internal interface IUnitOfWork
{
    Task<int> Complete();
}