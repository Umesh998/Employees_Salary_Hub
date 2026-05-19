using System.ComponentModel.DataAnnotations;

namespace Employees_Salary_Hub.ViewModels.Auth
{
    public class SetPasswordViewModel
    {
        [Required]
        [StringLength(100, MinimumLength = 8,
            ErrorMessage = "Password must be at least 8 characters.")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        // Passed via route or hidden field — identifies whose password is being set
        public string UserId { get; set; } = string.Empty;
    }
}


