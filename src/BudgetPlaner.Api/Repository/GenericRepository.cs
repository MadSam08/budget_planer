using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace BudgetPlaner.Api.Repository
{
    public class GenericRepository<TEntity, TContext>(TContext dbContext) : IGenericRepository<TEntity>
        where TEntity : class
        where TContext : DbContext, new()
    {
        private readonly DbSet<TEntity> _entities = dbContext.Set<TEntity>();

        public async Task AddAsync(TEntity entity)
        {
            await _entities.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await _entities.AddRangeAsync(entities);
        }

        public async Task UpdateAsync(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> props)
        {
            await _entities.Where(predicate).ExecuteUpdateAsync(props);
        }

        public async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            await _entities.Where(predicate).ExecuteDeleteAsync();
        }

        public Task<List<TEntity>> GetAllAsync()
        {
            return _entities.ToListAsync();
        }

        public Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return _entities.FirstOrDefaultAsync(predicate);
        }

        public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
        {
            return _entities.Where(predicate);
        }
    }
}