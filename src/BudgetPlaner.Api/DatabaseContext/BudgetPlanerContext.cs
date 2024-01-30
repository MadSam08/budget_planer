using BudgetPlaner.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace BudgetPlaner.Api.DatabaseContext;

public class BudgetPlanerContext : DbContext
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
        
        modelBuilder.Entity<CreditEntity>().HasKey(x => x.Id);
        modelBuilder.Entity<CreditEntity>()
            .HasOne(x => x.Currency)
            .WithMany()
            .HasForeignKey(c => c.CurrencyId);
        modelBuilder.Entity<CreditEntity>()
            .HasMany(x => x.InterestRates)
            .WithOne()
            .HasForeignKey(x => x.CreditId);

        modelBuilder.Entity<CreditInterestRate>().HasKey(x => x.Id);

        modelBuilder.Entity<CurrencyEntity>().HasKey(x => x.Id);
        modelBuilder.Entity<CurrencyEntity>().Property(x => x.Name).IsRequired();

        modelBuilder.Entity<IncomeEntity>().HasKey(x => x.Id);
        modelBuilder.Entity<IncomeEntity>().HasOne(x => x.Currency)
            .WithMany()
            .HasForeignKey(x => x.CurrencyId);
        modelBuilder.Entity<IncomeEntity>().HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId);
        
        modelBuilder.Entity<SpendingEntity>().HasKey(x => x.Id);
        modelBuilder.Entity<SpendingEntity>().HasOne(x => x.Currency)
            .WithMany()
            .HasForeignKey(x => x.CurrencyId);
        modelBuilder.Entity<SpendingEntity>().HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId);
        
        base.OnModelCreating(modelBuilder);
    }
}
