using System.ComponentModel.DataAnnotations;

namespace BudgetPlaner.Contracts.UI.Identity;

public record SignUpModel : SignInModel
{
    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", 
        ErrorMessage = "The password must be at least eight characters long and contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    [Display(Name = "Confirm User Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
