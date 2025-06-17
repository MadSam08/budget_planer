namespace BudgetPlaner.Contracts.Api.Profile;

using System.ComponentModel.DataAnnotations;

public record UserProfileModel
{
    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [StringLength(10)]
    public string PreferredCurrency { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal MonthlySavingsGoal { get; set; }
}
