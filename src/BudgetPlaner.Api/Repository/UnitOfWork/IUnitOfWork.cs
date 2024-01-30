using Microsoft.EntityFrameworkCore;

namespace BudgetPlaner.Api.Repository.UnitOfWork;

public interface IUnitOfWork<TContext>  where TContext : DbContext
{
    IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;

    Task<int> Complete();
}