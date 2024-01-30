using System.Collections;
using Microsoft.EntityFrameworkCore;

namespace BudgetPlaner.Api.Repository.UnitOfWork;

public sealed class UnitOfWork<TContext>(TContext context) : IUnitOfWork<TContext>, IDisposable
    where TContext : DbContext, new()
{
    private readonly Hashtable _repositories = [];

    public async Task<int> Complete()
    {
        return await context.SaveChangesAsync();
    }
    public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
    {
        var type = typeof(TEntity).Name;

        if (_repositories.ContainsKey(type)) return (IGenericRepository<TEntity>)_repositories[type]!;

        var repositoryType = typeof(GenericRepository<,>);
        var repositoryInstance =
            Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity), typeof(TContext)),
                context);

        _repositories.Add(type, repositoryInstance);

        return (IGenericRepository<TEntity>)_repositories[type]!;
    }

    public void Dispose()
    {
        Dispose(true);
    }

    private void Dispose(bool disposing)
    {
        if (!disposing) return;
        context.Dispose();
    }
}