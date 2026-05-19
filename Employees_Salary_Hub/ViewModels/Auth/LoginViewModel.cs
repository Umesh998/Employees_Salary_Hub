using System.ComponentModel.DataAnnotations;
namespace Employees_Salary_Hub.ViewModels.Auth
{
    public class LoginViewModel
    {
        [Required] public string EmployeeCode { get; set; } = string.Empty;
        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }

    public class OtpVerifyViewModel
    {
        [Required] public string UserId { get; set; } = string.Empty;
        [Required, StringLength(6, MinimumLength = 6)]
        public string OtpCode { get; set; } = string.Empty;
    }

  
}
