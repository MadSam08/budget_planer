using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace BudgetPlaner.Infrastructure.UnitOfWork;

public class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : DbContext
{
    private readonly TContext _context;
    private readonly ConcurrentDictionary<Type, object> _repositories;
    private bool _disposed;

    public UnitOfWork(TContext context)
    {
        _context = context;
        _repositories = new ConcurrentDictionary<Type, object>();
    }

    public TContext Context => _context;

    public IRepository<T> Repository<T>() where T : class
    {
        var type = typeof(T);
        
        return (IRepository<T>)_repositories.GetOrAdd(type, _ => new Repository<T>(_context));
    }

    public async Task<int> Complete()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _context.Dispose();
        }
        _disposed = true;
    }
} 