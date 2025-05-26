using Microsoft.EntityFrameworkCore;

namespace BudgetPlaner.Infrastructure.UnitOfWork;

public interface IUnitOfWork
{
    Task<int> Complete();
}

public interface IUnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
{
    IRepository<T> Repository<T>() where T : class;
    TContext Context { get; }
}