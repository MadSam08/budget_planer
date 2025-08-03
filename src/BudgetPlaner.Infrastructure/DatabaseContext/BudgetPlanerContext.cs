using BudgetPlaner.Domain;
using Microsoft.EntityFrameworkCore;

namespace BudgetPlaner.Infrastructure.DatabaseContext;

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
        modelBuilder.Entity<SpendingEntity>().HasOne(x => x.Budget)
            .WithMany()
            .HasForeignKey(x => x.BudgetId);
        modelBuilder.Entity<SpendingEntity>().HasOne(x => x.Currency)
            .WithMany()
            .HasForeignKey(x => x.CurrencyId);
        modelBuilder.Entity<SpendingEntity>().HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId);

        // Budget entities
        modelBuilder.Entity<BudgetEntity>().HasKey(x => x.Id);
        modelBuilder.Entity<BudgetEntity>().ToTable(TableNames.Budget);
        modelBuilder.Entity<BudgetEntity>().HasOne(x => x.Currency)
            .WithMany()
            .HasForeignKey(x => x.CurrencyId);
        modelBuilder.Entity<BudgetEntity>().HasMany(x => x.BudgetCategories)
            .WithOne(x => x.Budget)
            .HasForeignKey(x => x.BudgetId);

        modelBuilder.Entity<BudgetCategoryEntity>().HasKey(x => x.Id);
        modelBuilder.Entity<BudgetCategoryEntity>().ToTable(TableNames.BudgetCategory);
        modelBuilder.Entity<BudgetCategoryEntity>().HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId);

        // Savings entities
        modelBuilder.Entity<SavingsGoalEntity>().HasKey(x => x.Id);
        modelBuilder.Entity<SavingsGoalEntity>().ToTable(TableNames.SavingsGoal);
        modelBuilder.Entity<SavingsGoalEntity>().HasOne(x => x.Currency)
            .WithMany()
            .HasForeignKey(x => x.CurrencyId);
        modelBuilder.Entity<SavingsGoalEntity>().HasMany(x => x.Contributions)
            .WithOne(x => x.SavingsGoal)
            .HasForeignKey(x => x.SavingsGoalId);

        modelBuilder.Entity<SavingsContributionEntity>().HasKey(x => x.Id);
        modelBuilder.Entity<SavingsContributionEntity>().ToTable(TableNames.SavingsContribution);

        // Financial insights
        modelBuilder.Entity<FinancialInsightEntity>().HasKey(x => x.Id);
        modelBuilder.Entity<FinancialInsightEntity>().ToTable(TableNames.FinancialInsight);
        modelBuilder.Entity<FinancialInsightEntity>().HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId);

        // User profile
        modelBuilder.Entity<UserProfileEntity>().HasKey(x => x.Id);
        modelBuilder.Entity<UserProfileEntity>().ToTable(TableNames.UserProfile);

        // Loan payments
        modelBuilder.Entity<LoanPaymentEntity>().HasKey(x => x.Id);
        modelBuilder.Entity<LoanPaymentEntity>().ToTable(TableNames.LoanPayment);
        modelBuilder.Entity<LoanPaymentEntity>().HasOne(x => x.Loan)
            .WithMany(x => x.Payments)
            .HasForeignKey(x => x.LoanId);

        base.OnModelCreating(modelBuilder);
    }
}