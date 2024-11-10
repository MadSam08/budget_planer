using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace BudgetPlaner.Api.Repository;

public interface IGenericRepository<TEntity> where TEntity : class
{
    Task AddAsync(TEntity entity);

    Task AddRangeAsync(IEnumerable<TEntity> entities);

    Task UpdateAsync(Expression<Func<TEntity, bool>> predicate,
        Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> props);

    Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);
        
    Task<List<TEntity>> GetAllAsync();

    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

    IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate);
}