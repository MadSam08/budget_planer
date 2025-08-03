namespace BudgetPlaner.Contracts.Api.Budget;

public class BudgetResponse
{
    public string Id { get; set; }
    public string Name { get; set; }
    
    public string CurrencyName { get; set; }
    public decimal TotalBudgetAmount { get; set; }
    public BudgetPeriodType PeriodType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public BudgetStatus Status { get; set; }
    
}