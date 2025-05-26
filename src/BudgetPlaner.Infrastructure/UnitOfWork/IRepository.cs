using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace BudgetPlaner.Infrastructure.UnitOfWork;

public interface IRepository<T> where T : class
{
    // Query methods
    IQueryable<T> GetAll();
    IQueryable<T> Where(Expression<Func<T, bool>> predicate);
    Task<T?> GetByIdAsync(int id);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    Task<List<T>> ToListAsync();
    Task<List<T>> ToListAsync(CancellationToken cancellationToken);
    
    // Include methods for navigation properties
    IQueryable<T> Include<TProperty>(Expression<Func<T, TProperty>> navigationPropertyPath);
    IQueryable<T> Include(string navigationPropertyPath);
    
    // Add methods
    Task<T> AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    
    // Update methods
    void Update(T entity);
    void UpdateRange(IEnumerable<T> entities);
    Task UpdateAsync(Expression<Func<T, bool>> predicate, Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> setPropertyCalls);
    
    // Remove methods
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
    
    // Count methods
    Task<int> CountAsync();
    Task<int> CountAsync(Expression<Func<T, bool>> predicate);
    
    // Existence check
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
} 