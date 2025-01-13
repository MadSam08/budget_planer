using BudgetPlaner.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BudgetPlaner.Infrastructure.DatabaseContext;

public class IdentityContext(DbContextOptions<IdentityContext> options) : IdentityDbContext<IdentityUser>(options), IUnitOfWork
{
    public async Task<int> Complete()
    {
        return await base.SaveChangesAsync();
    }
}