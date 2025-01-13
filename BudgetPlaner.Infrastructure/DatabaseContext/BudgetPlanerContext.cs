using BudgetPlaner.Domain;
using BudgetPlaner.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace BudgetPlaner.Infrastructure.DatabaseContext;

public class BudgetPlanerContext : DbContext, IUnitOfWork
{
    public BudgetPlanerContext()
    {
        
    }
    
    public BudgetPlanerContext(DbContextOptions<BudgetPlanerContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CategoryEntity>().HasKey(x => x.Id);
        modelBuilder.Entity<CategoryEntity>().ToTable(TableNames.Category);
        
        modelBuilder.Entity<LoanEntity>().HasKey(x => x.Id);
        modelBuilder.Entity<LoanEntity>().ToTable(TableNames.Loan);
        modelBuilder.Entity<LoanEntity>()
            .HasOne(x => x.Currency)
            .WithMany()
            .HasForeignKey(c => c.CurrencyId);
        modelBuilder.Entity<LoanEntity>()
            .HasMany(x => x.ScheduledRates)
            .WithOne()
            .HasForeignKey(x => x.LoanId);

        modelBuilder.Entity<LoanInterestRateEntity>().HasKey(x => x.Id);
        modelBuilder.Entity<LoanInterestRateEntity>().ToTable(TableNames.LoanInterestRate);
        
        modelBuilder.Entity<CurrencyEntity>().HasKey(x => x.Id);
        modelBuilder.Entity<CurrencyEntity>().Property(x => x.Name).IsRequired();
        modelBuilder.Entity<CurrencyEntity>().ToTable(TableNames.Currency);

        modelBuilder.Entity<IncomeEntity>().HasKey(x => x.Id);
        modelBuilder.Entity<IncomeEntity>().ToTable(TableNames.Income);
        modelBuilder.Entity<IncomeEntity>().HasOne(x => x.Currency)
            .WithMany()
            .HasForeignKey(x => x.CurrencyId);
        modelBuilder.Entity<IncomeEntity>().HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId);
        
        modelBuilder.Entity<SpendingEntity>().HasKey(x => x.Id);
        modelBuilder.Entity<SpendingEntity>().ToTable(TableNames.Spending);
        modelBuilder.Entity<SpendingEntity>().HasOne(x => x.Currency)
            .WithMany()
            .HasForeignKey(x => x.CurrencyId);
        modelBuilder.Entity<SpendingEntity>().HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId);
        
        base.OnModelCreating(modelBuilder);
    }

    public async Task<int> Complete()
    {
        return await base.SaveChangesAsync();
    }
}
